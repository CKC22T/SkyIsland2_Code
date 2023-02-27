using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Olympus
{
    [RequireComponent(typeof(BoxCollider))]
    public class InteractiveGrassComputeObject : TextureTrackableObject
    {
        public enum INFLUENCE_MAP_TYPE
        {
            ALWAYS_CLEAR,
            CLEAR_AND_OUTER_FROM_PLAYER,
            NEVER_CLEAR,
        }

        public enum INFLUENCE_MAP_RESOLUTION
        {
            LOWEST_256,
            LOW_512,
            MEDIUM_1024,
            HIGH_2048,
            HIGHEST_4096,
        }

        public enum DISPATCHING_TYPE
        {
            SINGLE_1, 
            MINIMUM_2,
            LOW_4,
            MEDIUM_8,
            HIGH_16,
            MAXIMUM_32,
        }

        private ComputeShader mainCS;
        private RenderTexture uavTexture;
        private RenderTexture hostTexture;

        private SystemConstantData systemConstantBufferData;
        private ComputeBuffer systemConstantBuffer;

        [SerializeField] private List<GameObject> inSystemObjects = new();
        private List<ObjectData> objectStructuredBufferData = new();
        private Dictionary<GameObject, ObjectData> objectsPairs = new();
        private ComputeBuffer objectStructuredBuffer;
        private BoxCollider systemTriggerVolume;

        private Vector2Int mapResolution;
        public Vector3 anchorPoint = Vector3.zero;
        public Vector2 systemSize;
        public float effectRadius = 2.0f;
        public float vectorSmoothness = 2.0f;
        public int maximumObjectCount = 1024;
        public bool debugMode = false;
        public INFLUENCE_MAP_TYPE influenceFlag = INFLUENCE_MAP_TYPE.ALWAYS_CLEAR;
        public INFLUENCE_MAP_RESOLUTION resolution;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct ObjectData
        {
            public Vector4 positionDelta;
            public Vector4 position;
            public Vector4 direction;
            public Vector3 previousPosition;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 16)]
        struct SystemConstantData
        {
            public Vector4 anchor;
            public Vector2 systemSize;
            public Vector2Int resolution;

            public float radius;
            public float smoothness;

            public int mapType;
            public int objectCount;

            public Vector3Int blockDimension;
            public int unused_00;
            public Vector3Int gridDimension;
            public int unused_01;
        }

        private int systemConstantBufferSize;
        private int objectDataSize;
        private int mainKernelIndex;
        private int initKernelIndex;
        private int postKernelIndex;

        private Vector3Int dispatchSize;
        public DISPATCHING_TYPE dispatchType;
        private int threadsPerBlock;
        private int threadsPerInstanceChunk;
        private string initDispatchKernel = "InitKernel_";
        private string frameDispatchKernel = "InfluenceDrawKernel_";
        private string postDispatchKernel = "PostDrawKernel_";

        private void OnDestroy()
        {
            systemConstantBuffer.Dispose();

            RenderPipelineManager.endCameraRendering -= PostRenderScene;
        }

        void PreparePlayerInfluence()
        {
            mainCS = Resources.Load("Shaders/ComputeSample") as ComputeShader;
            MainTexture = new Texture2D(mapResolution.x, mapResolution.y, TextureFormat.RGBAHalf, false);
            hostTexture = Resources.Load("Shaders/InfluenceMap") as RenderTexture;

            dispatchSize = new Vector3Int(mapResolution.x / threadsPerBlock, mapResolution.y / threadsPerBlock, 1);

            RegenerateMap();

            if (uavTexture == null)
            {
                uavTexture = new RenderTexture(mapResolution.x, mapResolution.y, 1, UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SNorm)
                {
                    enableRandomWrite = true,
                    name = "uav texture",
                };

                uavTexture.Create();
            }
            LogUtil.Log("Compute Shader: " + mainCS.name);
            LogUtil.Log("Buffer Texture: " + uavTexture.name);

            systemConstantBufferData = new SystemConstantData();
            systemConstantBufferSize = System.Runtime.InteropServices.Marshal.SizeOf(systemConstantBufferData);
            systemConstantBuffer = new(1, systemConstantBufferSize, ComputeBufferType.Default, ComputeBufferMode.SubUpdates);

            ObjectData emptyData = new ObjectData();
            objectDataSize = System.Runtime.InteropServices.Marshal.SizeOf(emptyData);

            objectStructuredBuffer = new(maximumObjectCount, objectDataSize, ComputeBufferType.Structured, ComputeBufferMode.SubUpdates);

            systemConstantBufferData.blockDimension = new Vector3Int(threadsPerBlock, threadsPerBlock, 1);
            systemConstantBufferData.gridDimension = new Vector3Int(mapResolution.x / threadsPerBlock, mapResolution.y / threadsPerBlock, 1);

            var rawData = systemConstantBuffer.BeginWrite<SystemConstantData>(0, 1);
            rawData[0] = systemConstantBufferData;
            systemConstantBuffer.EndWrite<SystemConstantData>(1);

            var rawObjectData = objectStructuredBuffer.BeginWrite<ObjectData>(0, inSystemObjects.Count);
            for (int i = 0; i < inSystemObjects.Count; i++)
            {
                rawObjectData[i] = objectStructuredBufferData[i];
            }
            objectStructuredBuffer.EndWrite<ObjectData>(inSystemObjects.Count);

            int constantBufferId = Shader.PropertyToID("SystemConstantData");
            mainCS.SetConstantBuffer(constantBufferId, systemConstantBuffer, 0, systemConstantBufferSize);

            mainKernelIndex = mainCS.FindKernel(frameDispatchKernel);
            initKernelIndex = mainCS.FindKernel(initDispatchKernel);

            LogUtil.Log("InfluenceDrawKernel Kernel Index: " + mainKernelIndex);

            for (int i = 0; i < inSystemObjects.Count; i++)
            {
                ObjectData data = objectStructuredBufferData[i];
                data.position = inSystemObjects[i].transform.position;
            }

            mainCS.SetTexture(initKernelIndex, "Result", uavTexture);
            mainCS.Dispatch(initKernelIndex, dispatchSize.x, dispatchSize.y, 1);

        }
        void EvaluatePlayerInfluence()
        {
            mainKernelIndex = mainCS.FindKernel(frameDispatchKernel);
            initKernelIndex = mainCS.FindKernel(initDispatchKernel);

            systemConstantBufferData.radius = effectRadius;
            systemConstantBufferData.resolution = mapResolution;
            systemConstantBufferData.smoothness = vectorSmoothness;
            systemConstantBufferData.anchor = anchorPoint;
            systemConstantBufferData.mapType = (int)influenceFlag;
            systemConstantBufferData.systemSize = systemSize;
            systemConstantBufferData.blockDimension = new Vector3Int(threadsPerBlock, threadsPerBlock, 1);
            systemConstantBufferData.gridDimension = new Vector3Int(mapResolution.x / threadsPerBlock, mapResolution.y / threadsPerBlock, 1);
            systemConstantBufferData.objectCount = inSystemObjects.Count;

            for (int i = 0; i < inSystemObjects.Count; i++)
            {
                ObjectData data = objectStructuredBufferData[i];

                data.position = inSystemObjects[i].transform.position;
                data.positionDelta = (inSystemObjects[i].transform.position - data.previousPosition) * Time.deltaTime;

                objectStructuredBufferData[i] = data;
            }

            var rawData = systemConstantBuffer.BeginWrite<SystemConstantData>(0, 1);
            rawData[0] = systemConstantBufferData;
            systemConstantBuffer.EndWrite<SystemConstantData>(1);

            var rawObjectData = objectStructuredBuffer.BeginWrite<ObjectData>(0, inSystemObjects.Count);

            for (int i = 0; i < inSystemObjects.Count; i++)
            {
                rawObjectData[i] = objectStructuredBufferData[i];
            }

            objectStructuredBuffer.EndWrite<ObjectData>(inSystemObjects.Count);

            int constantBufferId = Shader.PropertyToID("SystemConstantData");
            int structuredBufferId = Shader.PropertyToID("ObjectsBuffer");

            mainCS.SetConstantBuffer(constantBufferId, systemConstantBuffer, 0, systemConstantBufferSize);
            mainCS.SetBuffer(mainKernelIndex, structuredBufferId, objectStructuredBuffer);
            mainCS.SetTexture(mainKernelIndex, "Result", uavTexture);

            mainCS.Dispatch(mainKernelIndex, dispatchSize.x, dispatchSize.y, dispatchSize.z);

            Graphics.CopyTexture(uavTexture, 0, 0, 0, 0, mapResolution.x, mapResolution.y, hostTexture, 0, 0, 0, 0);
            Graphics.CopyTexture(hostTexture, 0, 0, 0, 0, mapResolution.x, mapResolution.y, MainTexture, 0, 0, 0, 0);

            for (int i = 0; i < inSystemObjects.Count; i++)
            {
                ObjectData data = objectStructuredBufferData[i];
                Vector3 direction = (inSystemObjects[i].transform.position - data.previousPosition);

                if (direction.magnitude > 0.0f)
                {
                    data.direction = direction.normalized;
                }

                data.previousPosition = inSystemObjects[i].transform.position;
                objectStructuredBufferData[i] = data;
            }
        }

        void PostCompute()
        {
            postKernelIndex = mainCS.FindKernel(postDispatchKernel);
            mainCS.SetTexture(postKernelIndex, "Result", uavTexture);
            mainCS.Dispatch(postKernelIndex, dispatchSize.x, dispatchSize.y, 1);

        }

        private void Awake()
        {
            RenderPipelineManager.endCameraRendering += PostRenderScene;

            systemTriggerVolume = GetComponent<BoxCollider>();
            systemTriggerVolume.size = new Vector3(systemSize.x, 1.0F, systemSize.y);
            systemTriggerVolume.center = anchorPoint - transform.position;
        }

        void Start()
        {
            PreparePlayerInfluence();
        }

        void Update()
        {
            anchorPoint = transform.position;

            systemTriggerVolume.size = new Vector3(systemSize.x, 1.0f, systemSize.y);

            EvaluatePlayerInfluence();
        }

        void PostRenderScene(ScriptableRenderContext context, Camera camera)
        {
            PostCompute();
        }

        private void OnGUI()
        {
            if (debugMode == false)
            {
                return;
            }

            Camera cam = Camera.main;
            float aspectRatio = mapResolution.x / (float)mapResolution.y;
            Rect baseRect = new Rect(0, 0, 512, 512);
            if (UnityEngine.Event.current.type.Equals(EventType.Repaint))
            {
                GUI.DrawTexture(baseRect, hostTexture, ScaleMode.ScaleAndCrop, false);
                baseRect.height = 20;
                GUI.TextField(baseRect, "Scaled Space: " + new Vector2(systemSize.x / (float)mapResolution.x, systemSize.y / (float)mapResolution.y));

                baseRect.y += 20;
                GUI.TextField(baseRect, "Block Dimension: " + new Vector2(threadsPerBlock, threadsPerBlock));
                baseRect.y += 20;
                GUI.TextField(baseRect, "Grid Dimension: " + new Vector2(mapResolution.x / threadsPerBlock, mapResolution.y / threadsPerBlock));
            }
        }

        void RegenerateMap()
        {
            switch (resolution)
            {
                case INFLUENCE_MAP_RESOLUTION.LOWEST_256:
                    mapResolution = new Vector2Int(256, 256);
                    break;
                case INFLUENCE_MAP_RESOLUTION.LOW_512:
                    mapResolution = new Vector2Int(512, 512);
                    break;
                case INFLUENCE_MAP_RESOLUTION.MEDIUM_1024:
                    mapResolution = new Vector2Int(1024, 1024);
                    break;
                case INFLUENCE_MAP_RESOLUTION.HIGH_2048:
                    mapResolution = new Vector2Int(2048, 2048);
                    break;
                case INFLUENCE_MAP_RESOLUTION.HIGHEST_4096:
                    mapResolution = new Vector2Int(4096, 4096);
                    break;

            }

            dispatchSize = new Vector3Int(mapResolution.x / threadsPerBlock, mapResolution.y / threadsPerBlock);
            dispatchSize.z = (inSystemObjects.Count / threadsPerInstanceChunk);

            dispatchSize.z = Mathf.Clamp(dispatchSize.z, 1, maximumObjectCount);

            if (hostTexture != null && (hostTexture.width != mapResolution.x || hostTexture.height != mapResolution.y))
            {
                hostTexture.Release();
                MainTexture = null;

                hostTexture.width = mapResolution.x;
                hostTexture.height = mapResolution.y;
                hostTexture = Resources.Load("Shaders/InfluenceMap") as RenderTexture;

                if (MainTexture == null)
                {
                    MainTexture = new Texture2D(mapResolution.x, mapResolution.y, TextureFormat.RGBAHalf, false);
                }
            }

            if (uavTexture != null)
            {
                if (Application.isPlaying && (uavTexture.width != mapResolution.x || uavTexture.height != mapResolution.y))
                {
                    if (uavTexture != null)
                    {
                        uavTexture.Release();
                    }

                    uavTexture = null;

                    uavTexture = new RenderTexture(mapResolution.x, mapResolution.y, 1, UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SNorm)
                    {
                        enableRandomWrite = true
                    };

                    uavTexture.Create();
                }
            }
        }
        private void OnValidate()
        {
            initDispatchKernel = "InitKernel_ ";
            frameDispatchKernel = "InfluenceDrawKernel_ ";
            postDispatchKernel = "PostDrawKernel_ ";

            switch (dispatchType)
            {
                case DISPATCHING_TYPE.SINGLE_1:
                    threadsPerBlock = 1;
                    threadsPerInstanceChunk = 64;

                    frameDispatchKernel = frameDispatchKernel.Substring(0, frameDispatchKernel.LastIndexOf('_') + 1);
                    frameDispatchKernel += "SINGLE";

                    postDispatchKernel = postDispatchKernel.Substring(0, postDispatchKernel.LastIndexOf('_') + 1);
                    postDispatchKernel += "SINGLE";

                    initDispatchKernel = initDispatchKernel.Substring(0, initDispatchKernel.LastIndexOf('_') + 1);
                    initDispatchKernel += "SINGLE";

                    break;
                case DISPATCHING_TYPE.MINIMUM_2:
                    threadsPerBlock = 2;
                    threadsPerInstanceChunk = 64;

                    frameDispatchKernel = frameDispatchKernel.Substring(0, frameDispatchKernel.LastIndexOf('_') + 1);
                    frameDispatchKernel += "MINIMUM";

                    postDispatchKernel = postDispatchKernel.Substring(0, postDispatchKernel.LastIndexOf('_') + 1);
                    postDispatchKernel += "MINIMUM";

                    initDispatchKernel = initDispatchKernel.Substring(0, initDispatchKernel.LastIndexOf('_') + 1);
                    initDispatchKernel += "MINIMUM";
                    break;
                case DISPATCHING_TYPE.LOW_4:
                    threadsPerBlock = 4;
                    threadsPerInstanceChunk = 32;
                    frameDispatchKernel = frameDispatchKernel.Substring(0, frameDispatchKernel.LastIndexOf('_') + 1);
                    frameDispatchKernel += "LOW";

                    postDispatchKernel = postDispatchKernel.Substring(0, postDispatchKernel.LastIndexOf('_') + 1);
                    postDispatchKernel += "LOW";

                    initDispatchKernel = initDispatchKernel.Substring(0, initDispatchKernel.LastIndexOf('_') + 1);
                    initDispatchKernel += "LOW";
                    break;
                case DISPATCHING_TYPE.MEDIUM_8:
                    threadsPerBlock = 8;
                    threadsPerInstanceChunk = 16;
                    frameDispatchKernel = frameDispatchKernel.Substring(0, frameDispatchKernel.LastIndexOf('_') + 1);
                    frameDispatchKernel += "MEDIUM";

                    postDispatchKernel = postDispatchKernel.Substring(0, postDispatchKernel.LastIndexOf('_') + 1);
                    postDispatchKernel += "MEDIUM";

                    initDispatchKernel = initDispatchKernel.Substring(0, initDispatchKernel.LastIndexOf('_') + 1);
                    initDispatchKernel += "MEDIUM";
                    break;
                case DISPATCHING_TYPE.HIGH_16:
                    threadsPerBlock = 16;
                    threadsPerInstanceChunk = 4;
                    frameDispatchKernel = frameDispatchKernel.Substring(0, frameDispatchKernel.LastIndexOf('_') + 1);
                    frameDispatchKernel += "HIGH";

                    postDispatchKernel = postDispatchKernel.Substring(0, postDispatchKernel.LastIndexOf('_') + 1);
                    postDispatchKernel += "HIGH";

                    initDispatchKernel = initDispatchKernel.Substring(0, initDispatchKernel.LastIndexOf('_') + 1);
                    initDispatchKernel += "HIGH";
                    break;
                case DISPATCHING_TYPE.MAXIMUM_32:
                    threadsPerBlock = 32;
                    threadsPerInstanceChunk = 1;
                    frameDispatchKernel = frameDispatchKernel.Substring(0, frameDispatchKernel.LastIndexOf('_') + 1);
                    frameDispatchKernel += "MAXIMUM";

                    postDispatchKernel = postDispatchKernel.Substring(0, postDispatchKernel.LastIndexOf('_') + 1);
                    postDispatchKernel += "MAXIMUM";

                    initDispatchKernel = initDispatchKernel.Substring(0, initDispatchKernel.LastIndexOf('_') + 1);
                    initDispatchKernel += "MAXIMUM";
                    break;
            }
            RegenerateMap();
        }

        private void OnDrawGizmos()
        {
            if (debugMode == true)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(anchorPoint, new Vector3(systemSize.x, 0.0f, systemSize.y));
                if (debugMode == true)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapResolution.x, 0, mapResolution.y));
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (debugMode == false)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(anchorPoint, new Vector3(systemSize.x, 0.0f, systemSize.y));
                if (debugMode == true)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapResolution.x, 0, mapResolution.y));
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("GrassField") == true)
            {
                return;
            }
            if (inSystemObjects.Contains(other.gameObject) == false)
            {
                inSystemObjects.Add(other.gameObject);
                ObjectData data = new ObjectData();
                objectsPairs.Add(other.gameObject, data);
                objectStructuredBufferData.Add(data);
            }

            dispatchSize.z = (inSystemObjects.Count / threadsPerInstanceChunk);
            dispatchSize.z = Mathf.Clamp(dispatchSize.z, 1, maximumObjectCount);
        }

        private void OnTriggerExit(Collider other)
        {
            if (inSystemObjects.Contains(other.gameObject) == true)
            {
                inSystemObjects.Remove(other.gameObject);
                objectStructuredBufferData.Remove(objectsPairs.GetValueOrDefault(other.gameObject));
                objectsPairs.Remove(other.gameObject);
            }
            dispatchSize.z = (inSystemObjects.Count / threadsPerInstanceChunk);
            dispatchSize.z = Mathf.Clamp(dispatchSize.z, 1, maximumObjectCount);
        }
    }
}