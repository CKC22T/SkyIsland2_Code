using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using Unity.VisualScripting;

namespace Olympus
{
    public class PlayerCameraExtension : EditorWindow
    {
        string guiTargetShake;
        public bool trackTarget;
        static public PlayerCameraExtension instance;
        private PlayerCamera cameraInstance;
        private Rect mainRect;
        [MenuItem("Window/Camera Shake Editor")]
        public static void CreateWindow()
        {
            if (instance == null)
            {
                instance = GetWindow<PlayerCameraExtension>();
            }
            instance.cameraInstance = Camera.main.GetComponent<PlayerCamera>();
            instance.cameraInstance.Awake();
            instance.cameraInstance.Start();
        }

        public void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (trackTarget == true)
            {
                cameraInstance.Tracking(Time.deltaTime);
            }
        }

        int selectedProfile = 0;
        List<string> profileNames = new();
        PlayerCamera.CameraShakeDescriptor selectedDescriptor;
        public Dictionary<string, AnimationCurve> cachedCurves = new();
        bool showCurve = false;
        private float tangent = 0.0f;
        public void UpdateWave(string name)
        {
            cachedCurves.Remove(name);

            AnimationCurve sampleCurve = new AnimationCurve();
            PlayerCamera.CameraShakeDescriptor descriptor = cameraInstance.cameraShakeList[name];

            int points = 30 * (int)descriptor.frequency;
            int index = 0;
            float delta = descriptor.duration;
            float y = 0.0f;

            while (index < points)
            {
                y = Mathf.Sin(delta * descriptor.frequency) * (descriptor.magnitude * (descriptor.duration - delta));

                delta -= Time.deltaTime;
                sampleCurve.AddKey(delta, y);

                if (delta < 0.0f)
                {
                    break;
                }

                index++;
            }

            if (cachedCurves.ContainsKey(name) == false)
            {
                cachedCurves.Add(name, sampleCurve);
            }

        }

        public void GenerateWaves()
        {
            cachedCurves.Clear();
            for (int i = 0; i < profileNames.Count; i++)
            {
                AnimationCurve sampleCurve = new AnimationCurve();
                PlayerCamera.CameraShakeDescriptor descriptor = cameraInstance.cameraShakeList[profileNames[i]];

                int index = 0;
                bool upward = true;
                float frameTime = descriptor.duration;
                float delta = 0.0f;
                float y = 0.0f;
                float previous = 0.0f;

                while (frameTime > 0.0f)
                {
                    y = Mathf.Sin(frameTime * descriptor.frequency) * (descriptor.magnitude * (descriptor.duration - frameTime));
                    delta = y - previous;

                    Keyframe key = new Keyframe(frameTime, previous);

                    if (y == 0.0f)
                    {
                        sampleCurve.AddKey(key);
                    }
                    if (delta > 0.0f)
                    {
                        // record minimum
                        if (upward == false)
                        {
                            sampleCurve.AddKey(key);
                        }
                        upward = true;

                    }
                    else if (delta < 0.0f)
                    {
                        if (upward == true)
                        {
                            sampleCurve.AddKey(key);
                        }
                        upward = false;

                    }

                    previous = y;
                    index++;
                    frameTime -= 0.0001f;
                }

                if (cachedCurves.ContainsKey(profileNames[i]) == false)
                {
                    cachedCurves.Add(profileNames[i], sampleCurve);
                }
            }

            Repaint();
        }

        public void OnGUI()
        {
            if (instance == null)
            {
                return;
            }
            trackTarget = GUILayout.Toggle(trackTarget, "Tracking Target");
            showCurve = GUILayout.Toggle(showCurve, "Show Curvature");
            if (GUILayout.Button("Refresh List") == true)
            {
                profileNames.Clear();
                int index = 0;
                foreach (var i in cameraInstance.cameraShakeList)
                {
                    profileNames.Add(i.Key);
                    index++;
                }

                GenerateWaves();
            }

            GUILayout.Label("Cam Shake Profiles");

            selectedProfile = GUILayout.SelectionGrid(selectedProfile, profileNames.ToArray(), 1, GUILayout.Width(160.0f));

            if (GUILayout.Button("Play") == true)
            {
                if (cameraInstance.cameraShakeList.TryGetValue(profileNames[selectedProfile], out selectedDescriptor) == false)
                {
                    LogUtil.LogError("Invalid argument, no such descriptor was found, " + guiTargetShake);
                    return;
                }
                //EditorCoroutineUtility.StartCoroutineOwnerless(cameraInstance.ShakeCamera(selectedDescriptor));
                EditorCoroutineUtility.StartCoroutineOwnerless(cameraInstance.ShakeCamera(selectedDescriptor));
            }

            if (showCurve == true)
            {
                if (selectedProfile < profileNames.Count)
                {
                    if (cachedCurves.ContainsKey(profileNames[selectedProfile]) == true)
                    {
                        EditorGUI.CurveField(EditorGUILayout.GetControlRect(), cachedCurves[profileNames[selectedProfile]]);
                    }
                }
            }

            tangent = GUILayout.HorizontalSlider(tangent, -64.0f, 16.0f);
            GUILayout.Label(new GUIContent(tangent.ToString()));


        }
    }
}
#endif