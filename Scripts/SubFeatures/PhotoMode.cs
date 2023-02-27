using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using UnityEngine.UI;

namespace Olympus
{
    [RequireComponent(typeof(Camera))]
    public class PhotoMode : SingletonBase<PhotoMode>
    {
        float fieldOfView;
        bool resumeFlag = true;

        private Texture2D capturedBuffer;
        private RenderTexture captureRenderTarget;
        public float cameraMovementSpeed;
        private float initialCameraMovementSpeed;
        public float cameraRotationalSpeed;

        readonly string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/My Games/SkyIslands/Screenshots/";
        readonly string fileName = "CAPTURED_IMAGE_";

        private List<UIBase> uiBuffer = new();

        PlayerCamera playerCameraInstance;

        [SerializeField] private GameObject interfaceRootObject;
        [SerializeField] private Volume postProcessVolume;
        public Volume PostProcessVolume {
            get { return postProcessVolume; }
            set { postProcessVolume = value; }
        }

        [SerializeField] private Camera targetCamera;
        private BoxCollider cameraCollider;
        private Rigidbody cameraRigidbody;

        public Camera TargetCamera {
            get { return Camera.main; }
        }
        [SerializeField] private VolumeProfile postProcessProfile;
        public VolumeProfile PostProcessProfile {
            get { return postProcessProfile; }
            set { postProcessProfile = value; }
        }

        [SerializeField] private Dictionary<int, Volume> volumeContainer = new();
        static private readonly System.Type[] supportedPostProcessElements = { typeof(DepthOfField), typeof(Vignette), typeof(Bloom), typeof(Tonemapping), typeof(LiftGammaGain), typeof(WhiteBalance), typeof(MotionBlur) };
        private List<VolumeComponent> postProcessEffectComponents = new(supportedPostProcessElements.Length);

        static private readonly int initialPostProcessComponentDictionarySize = 32;
        private Dictionary<int, DepthOfField> componentDepthOfFields = new(initialPostProcessComponentDictionarySize);
        private Dictionary<int, Vignette> componentVignettes = new(initialPostProcessComponentDictionarySize);
        private Dictionary<int, WhiteBalance> componentWhiteBalances = new(initialPostProcessComponentDictionarySize);
        private Dictionary<int, LiftGammaGain> componentLiftGammaGains = new(initialPostProcessComponentDictionarySize);
        private Dictionary<int, Tonemapping> componentTonemappings = new(initialPostProcessComponentDictionarySize);
        private Dictionary<int, Bloom> componentBlooms = new(initialPostProcessComponentDictionarySize);
        private Dictionary<int, MotionBlur> componentMotionBlurs = new(initialPostProcessComponentDictionarySize);

        private DepthOfField currentDepthOfFieldInstance;
        private Vignette currentVignetteInstance;
        private WhiteBalance currentWhiteBalanceInstance;
        private LiftGammaGain currentLiftGammaGainInstance;
        private Tonemapping currentTonemappingInstance;
        private Bloom currentBloomInstance;
        private MotionBlur currentMotionBlurInstance;

        private DepthOfField previousDepthOfFieldInstance;
        private Vignette previousVignetteInstance;
        private WhiteBalance previousWhiteBalanceInstance;
        private LiftGammaGain previousLiftGammaGainInstance;
        private Tonemapping previousTonemappingInstance;
        private Bloom previousBloomInstance;
        private MotionBlur previousMotionBlurInstance;

        private List<UIBase> photoModeUIList = new();

        private Vector3 previousMousePosition = Vector3.zero;
        private Vector3 mouseDelta = Vector3.zero;
        public float sensitivity = 2.0f;

        public float scrollSensitivity = 2.0f;
        public float maximumFov = 90.0f;
        public float minimumFov = 30.0f;

        public Volume globalVolume;
        public Material playerMeshMaterial;
        [SerializeField] Material[] playerMaterials;
        public SkinnedMeshRenderer playerSkinnedMeshRenderer;

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            playerSkinnedMeshRenderer.materials = playerMaterials;
            playerMeshMaterial = playerSkinnedMeshRenderer?.material;
            int ditherID = Shader.PropertyToID("_Dither_Switch");
            playerMeshMaterial.SetFloat(ditherID, 0.0f);

            //UIManager.Show(UIList.CharacterHPUI);
            ////UIManager.Show<QuestInfoUI>(UIList.QuestInfoUI);
            //UIManager.Show(UIList.ItemSlotUI);
            //UIManager.Show(UIList.SkillUI);
            //UIManager.Show(UIList.EnemyHUDManagerUI);
            //UIManager.Hide(UIList.PhotoModeUI);


            var sceneEntities = GameManager.Instance.SceneEntityContainer;
            var controllerBuffer = GameManager.Instance.ControllerBuffer;

            //for (int i = 0; i < sceneEntities.Count; i++)
            //{
            //    var previousController = controllerBuffer.GetValueOrDefault(sceneEntities[i].GetHashCode());
            //    bool flagRemove = controllerBuffer.Remove(sceneEntities[i].GetHashCode());
            //    LogUtil.Assert(flagRemove == true);

            //    sceneEntities[i].ChangeEntityController(previousController);
            //}

            // UIManager.DisplayGroup("GamePlay", true);
            UIManager.RecoverPreviousStatus();
            UIManager.DisplayGroup("PhotoMode", false);

            if (currentDepthOfFieldInstance == null || currentVignetteInstance == null || currentWhiteBalanceInstance == null || currentLiftGammaGainInstance == null || currentTonemappingInstance == null | currentBloomInstance == null || currentMotionBlurInstance == null)
            {
                return;
            }

            currentDepthOfFieldInstance.Override(previousDepthOfFieldInstance, 1.0f);
            currentVignetteInstance.Override(previousVignetteInstance, 1.0f);
            currentWhiteBalanceInstance.Override(previousWhiteBalanceInstance, 1.0f);
            currentLiftGammaGainInstance.Override(previousLiftGammaGainInstance, 1.0f);
            currentTonemappingInstance.Override(previousTonemappingInstance, 1.0f);
            currentBloomInstance.Override(previousBloomInstance, 1.0f);
            currentMotionBlurInstance.Override(previousMotionBlurInstance, 1.0f);
        }

        private void OnEnable()
        {
            SoundManager.Instance.PlaySound("UI_Camera");
            Cursor.lockState = CursorLockMode.Locked;
            playerSkinnedMeshRenderer = PlayerController.Instance.PlayerEntity.EntityMeshRenderers[0];

            if (currentDepthOfFieldInstance != null)
            {
                currentDepthOfFieldInstance.focalLength.value = 70.0f;
            }

            playerSkinnedMeshRenderer.materials = new Material[1];
            playerSkinnedMeshRenderer.material = playerMaterials[0];
            playerMeshMaterial = playerSkinnedMeshRenderer.material;
            int ditherID = Shader.PropertyToID("_Dither_Switch");
            playerMeshMaterial.SetFloat(ditherID, 1.0f);

            //UIManager.Show(UIList.PhotoModeUI);
            //UIManager.Hide(UIList.CharacterHPUI);
            //UIManager.Hide(UIList.QuestInfoUI);

            var sceneEntities = GameManager.Instance.SceneEntityContainer;
            var controllerBuffer = GameManager.Instance.ControllerBuffer;

            //for (int i = 0; i < sceneEntities.Count; i++)
            //{
            //    controllerBuffer.Add(sceneEntities[i].GetHashCode(), sceneEntities[i].EntityController);
            //    sceneEntities[i].EntityData.moveDirection = Vector3.zero;

            //    sceneEntities[i].ChangeEntityController(NullController.Instance);
            //}

            targetCamera.fieldOfView = maximumFov;

            UIManager.SaveCurrentStates();
            UIManager.DisplayGroup("PhotoMode", true);

            if (currentDepthOfFieldInstance == null || currentVignetteInstance == null || currentWhiteBalanceInstance == null || currentLiftGammaGainInstance == null || currentTonemappingInstance == null | currentBloomInstance == null || currentMotionBlurInstance == null)
            {
                return;
            }

            previousDepthOfFieldInstance.Override(currentDepthOfFieldInstance, 1.0f);
            previousVignetteInstance.Override(currentVignetteInstance, 1.0f);
            previousWhiteBalanceInstance.Override(currentWhiteBalanceInstance, 1.0f);
            previousLiftGammaGainInstance.Override(currentLiftGammaGainInstance, 1.0f);
            previousTonemappingInstance.Override(currentTonemappingInstance, 1.0f);
            previousBloomInstance.Override(currentBloomInstance, 1.0f);
            previousMotionBlurInstance.Override(currentMotionBlurInstance, 1.0f);
        }
        Texture2D ToTexture2D(RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
            // ReadPixels looks at the active RenderTexture.
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            return tex;
        }
        protected override void Awake()
        {
            playerCameraInstance = Camera.main.GetComponent<PlayerCamera>();

            previousDepthOfFieldInstance = ScriptableObject.CreateInstance("DepthOfField") as DepthOfField;
            previousVignetteInstance = ScriptableObject.CreateInstance("Vignette") as Vignette;
            previousWhiteBalanceInstance = ScriptableObject.CreateInstance("WhiteBalance") as WhiteBalance;
            previousLiftGammaGainInstance = ScriptableObject.CreateInstance("LiftGammaGain") as LiftGammaGain;
            previousTonemappingInstance = ScriptableObject.CreateInstance("Tonemapping") as Tonemapping;
            previousBloomInstance = ScriptableObject.CreateInstance("Bloom") as Bloom;
            previousMotionBlurInstance = ScriptableObject.CreateInstance("MotionBlur") as MotionBlur;

            cameraCollider = GetComponent<BoxCollider>();
            cameraRigidbody = GetComponent<Rigidbody>();
            targetCamera = Camera.main;

            initialCameraMovementSpeed = cameraMovementSpeed;
            cameraRigidbody.isKinematic = false;

            photoModeUIList.Add(UIManager.Instance.GetUI(UIList.PhotoModeUI));

            //  postProcessProfile = globalVolume.profile;
            globalVolume.profile.TryGet<DepthOfField>(out currentDepthOfFieldInstance);

            enabled = false;

            layerMask = ~(1 << LayerMask.NameToLayer("Player"));
        }

        int layerMask;
        float verticalAngle = 0.0f, horizontalAngle = 0.0f;

        public void Update()
        {
            if (currentDepthOfFieldInstance == null)
            {
                postProcessProfile = globalVolume.profile;
                globalVolume.profile.TryGet<DepthOfField>(out currentDepthOfFieldInstance);
            }

            Ray r = new Ray(transform.position, transform.forward);
            RaycastHit hitResult;

            bool hit = Physics.Raycast(r, out hitResult, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore);
            Transform closestObject = null;
            Vector3 focusPoint = transform.forward;

            if (hit == true)
            {
                closestObject = hitResult.collider.transform;
                focusPoint = hitResult.point;
                Debug.DrawLine(transform.position, focusPoint, Color.red);
            }

            if (closestObject != null)
            {
                float distance = Vector3.Distance(transform.position, focusPoint);
                //float length = targetCamera.fieldOfView * 2.0f;

                currentDepthOfFieldInstance.focusDistance.value = Mathf.Lerp(currentDepthOfFieldInstance.focusDistance.value, distance, 0.2f);
                // currentDepthOfFieldInstance.focalLength.value = 360.0f / (Mathf.Deg2Rad * targetCamera.fieldOfView * 2);
                //    LogUtil.Log(length);
                // UnityEditor.Handles.DrawWireCube(closestObject.position, new Vector3(1, 2, 1));
            }

            mouseDelta = previousMousePosition - Input.mousePosition;
            cameraMovementSpeed = Input.GetKey(KeyCode.LeftShift) == true ? initialCameraMovementSpeed * 2 : initialCameraMovementSpeed;

            Vector3 playerHeadPoint = PlayerController.Instance.PlayerEntity.transform.position + new Vector3(0, 4.0f, 0);

            transform.position = Vector3.Lerp(transform.position, playerHeadPoint, 1.0f);

            verticalAngle += -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            horizontalAngle += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

            verticalAngle = Mathf.Clamp(verticalAngle, -90.0f, 90.0f);

            transform.rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0.0f);

            if (Input.GetKey(KeyCode.LeftBracket) == true)
            {
                currentDepthOfFieldInstance.focusDistance.value -= 2.0f * Time.deltaTime;

                currentDepthOfFieldInstance.focusDistance.value = Mathf.Clamp(currentDepthOfFieldInstance.focusDistance.value, 0.0f, 300.0f);
            }

            if (Input.GetKey(KeyCode.RightBracket) == true)
            {
                currentDepthOfFieldInstance.focusDistance.value += 2.0f * Time.deltaTime;

                currentDepthOfFieldInstance.focusDistance.value = Mathf.Clamp(currentDepthOfFieldInstance.focusDistance.value, 0.0f, 300.0f);
            }

            if (Input.GetMouseButtonDown(0) == true)
            {
                Capture();
                LogUtil.Log("Capture Event Detected");
            }

            if (Mathf.Abs(Input.mouseScrollDelta.y) >= 1)
            {
                float delta = Input.mouseScrollDelta.y / 10.0f * scrollSensitivity;

                targetCamera.fieldOfView -= delta;
                targetCamera.fieldOfView = Mathf.Clamp(targetCamera.fieldOfView, minimumFov, maximumFov);
            }

            previousMousePosition = Input.mousePosition;
        }

        static public Texture2D GetActiveFrame(RenderTexture rt)
        {
            RenderTexture currentActiveRT = RenderTexture.active;

            RenderTexture.active = rt;

            Texture2D tex = new Texture2D(rt.width, rt.height);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

            RenderTexture.active = currentActiveRT;
            return tex;
        }
        public void Capture()
        {
            SoundManager.Instance.PlaySound("UI_Filming");

            string fullDirectory = "";
            int index = 0;
            captureRenderTarget = new RenderTexture(Screen.width, Screen.height, 1);

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            while (true)
            {
                string timeString = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");
                fullDirectory = path + timeString + ".png";

                if (File.Exists(fullDirectory) == true)
                {
                    index++;
                }
                else
                {
                    break;
                }
            }

            UIManager.Hide(UIList.PhotoModeUI);

            //int width = Screen.width;
            //int height = Screen.height;

            //Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);

            //tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            //tex.Apply();

            //GameManager.Instance.AddPhoto(tex);

            //UIManager.Show(UIList.PhotoModeUI);
            int width = Screen.width;
            int height = Screen.height;

            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);

            //ScreenCapture.CaptureScreenshot(fullDirectory);
            StartCoroutine(CaptureAsync(tex, width, height, fullDirectory));

            StartCoroutine(WaitForCaptureTaskAsync(fullDirectory));
        }

        void SaveCapture(string directory, Texture2D tex)
        {
            byte[] texels = tex.EncodeToPNG();

            File.WriteAllBytes(directory, texels);
        }

        IEnumerator CaptureAsync(Texture2D tex, int width, int height, string directory)
        {
            yield return new WaitForEndOfFrame();

            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            SaveCapture(directory, tex);

            GameManager.Instance.AddPhoto(tex);
        }

        IEnumerator WaitForCaptureTaskAsync(string path)
        {
            yield return new WaitForEndOfFrame();
            while (true)
            {
                if (File.Exists(path) == true)
                {
                    break;
                }

                yield return null;
            }

            UIManager.Show(UIList.PhotoModeUI);

            yield return null;
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.CompareTag("PPVolume") == true)
        //    {
        //        LogUtil.Log(other.gameObject.name);

        //        var ppv = volumeContainer.GetValueOrDefault(other.gameObject.GetHashCode());

        //        LogUtil.Assert(ppv != null, "Invalid ppv, check ppv tag on inspector");
        //        postProcessVolume = ppv;

        //        LogUtil.Assert(ppv.profile != null, "Invalid ppv profile, check asset named ' " + ppv.name + " ' ");
        //        postProcessProfile = ppv.profile;

        //        if (volumeContainer.ContainsKey(ppv.GetHashCode()) == false)
        //        {
        //            volumeContainer.Add(ppv.GetHashCode(), ppv);

        //            int hash = postProcessProfile.GetHashCode();
        //            if (componentDepthOfFields.ContainsKey(hash) == false)
        //            {
        //                DepthOfField instance;
        //                postProcessProfile.TryGet<DepthOfField>(out instance);

        //                componentDepthOfFields[hash] = instance;
        //            }
        //            if (componentVignettes.ContainsKey(hash) == false)
        //            {
        //                Vignette instance;
        //                postProcessProfile.TryGet<Vignette>(out instance);

        //                componentVignettes[hash] = instance;
        //            }
        //            if (componentBlooms.ContainsKey(hash) == false)
        //            {
        //                Bloom instance;
        //                postProcessProfile.TryGet<Bloom>(out instance);

        //                componentBlooms[hash] = instance;
        //            }
        //            if (componentTonemappings.ContainsKey(hash) == false)
        //            {
        //                Tonemapping instance;
        //                postProcessProfile.TryGet<Tonemapping>(out instance);

        //                componentTonemappings[hash] = instance;
        //            }
        //            if (componentLiftGammaGains.ContainsKey(hash) == false)
        //            {
        //                LiftGammaGain instance;
        //                postProcessProfile.TryGet<LiftGammaGain>(out instance);

        //                componentLiftGammaGains[hash] = instance;
        //            }
        //            if (componentWhiteBalances.ContainsKey(hash) == false)
        //            {
        //                WhiteBalance instance;
        //                postProcessProfile.TryGet<WhiteBalance>(out instance);

        //                componentWhiteBalances[hash] = instance;
        //            }
        //            if (componentMotionBlurs.ContainsKey(hash) == false)
        //            {
        //                MotionBlur instance;
        //                postProcessProfile.TryGet<MotionBlur>(out instance);

        //                componentMotionBlurs[hash] = instance;
        //            }
        //        }

        //        OnVolumeChange(postProcessProfile);
        //    }
        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    globalVolume.profile.TryGet<DepthOfField>(out currentDepthOfFieldInstance);
        //}
        //private void OnVolumeChange(VolumeProfile profile)
        //{
        //    postProcessEffectComponents.Clear();
        //    int hash = postProcessProfile.GetHashCode();

        //    componentDepthOfFields.TryGetValue(hash, out currentDepthOfFieldInstance);
        //    componentVignettes.TryGetValue(hash, out currentVignetteInstance);
        //    componentBlooms.TryGetValue(hash, out currentBloomInstance);
        //    componentTonemappings.TryGetValue(hash, out currentTonemappingInstance);
        //    componentLiftGammaGains.TryGetValue(hash, out currentLiftGammaGainInstance);
        //    componentWhiteBalances.TryGetValue(hash, out currentWhiteBalanceInstance);
        //    componentMotionBlurs.TryGetValue(hash, out currentMotionBlurInstance);
        //}
    }
}