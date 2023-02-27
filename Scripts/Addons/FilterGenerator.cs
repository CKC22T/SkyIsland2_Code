using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.Rendering.Universal;

namespace Olympus
{
    public class FilterGenerator : EditorWindow
    {
        static FilterGenerator instance;

        //PhotoMode.Filter filterParameters;
        private bool previewToggle;
        PhotoMode photoModeInstance;

        private string fileName;

        private UnityEngine.Rendering.VolumeProfile openedProfile;
        private UnityEngine.Rendering.VolumeProfile tempProfile;

        private Vignette openedProfileVignette;
        private WhiteBalance openedProfileWhiteBalance;
        private FilmGrain openedProfileFilmGrain;
        private Tonemapping openedProfileTonemapping;
        private ColorAdjustments openedProfileColorAdjustment;

        GUIContent[] colorLabels = { new GUIContent("R"), new GUIContent("G"), new GUIContent("B"), new GUIContent("A") };

        [MenuItem("Window/Filter Generator")]
        static private void CreateWindow()
        {
            instance = EditorWindow.GetWindow<FilterGenerator>();
        }

        private void OnEnable()
        {
            tempProfile = null;
            autoRepaintOnSceneChange = true;

            
        }

        private void OnDisable()
        {
            previewToggle = false;
            PreviewSwitch(false);
        }
        private void OnDestroy()
        {
            previewToggle = false;
            PreviewSwitch(false);
        }

        void PreviewSwitch(bool flag)
        {
            if (previewToggle == true)
            {
                tempProfile = photoModeInstance.PostProcessProfile;
                photoModeInstance.PostProcessVolume.profile = openedProfile;
            }
            else
            {
                photoModeInstance.PostProcessVolume.profile = tempProfile;
                tempProfile = null;
            }
        }

        private void OnGUI()
        {
            if (photoModeInstance == null)
            {
                photoModeInstance = Camera.main.GetComponent<PhotoMode>();
            }

            if(photoModeInstance.PostProcessVolume == null)
            {
                photoModeInstance.PostProcessVolume = Camera.main.GetComponent<UnityEngine.Rendering.Volume>();
            }

            Rect rect = EditorGUILayout.GetControlRect();

            openedProfile = EditorGUI.ObjectField(rect, openedProfile, typeof(UnityEngine.Rendering.VolumeProfile), true) as UnityEngine.Rendering.VolumeProfile;
            rect.y += 20;

            fileName = EditorGUI.TextArea(rect, fileName);
            rect.y += 20;
            if (GUI.Button(rect, "Create New Post-Process Profile"))
            {
                int index = 0;
                UnityEngine.Rendering.VolumeProfile profile = new();

                string filePath = "Assets/Olympus/Art/PostProcesses/Profiles/" + fileName + "_" + index + ".asset";
                string fullPath = Application.dataPath + filePath;
                LogUtil.Log(fullPath);

                while (true)
                {
                    if (File.Exists(fullPath) == true)
                    {
                        index++;
                        filePath = "Assets/Olympus/Art/PostProcesses/Profiles/" + fileName + "_" + index + ".asset";
                        fullPath = Application.dataPath + "/Olympus/Art/PostProcesses/Profiles/" + fileName + "_" + index + ".asset"; ;
                    }
                    else
                    {
                        break;
                    }
                }

                openedProfileVignette = profile.Add(typeof(Vignette)) as Vignette;
                openedProfileWhiteBalance = profile.Add(typeof(WhiteBalance)) as WhiteBalance;
                openedProfileFilmGrain = profile.Add(typeof(FilmGrain)) as FilmGrain;
                openedProfileTonemapping = profile.Add(typeof(Tonemapping)) as Tonemapping;
                openedProfileColorAdjustment = profile.Add(typeof(ColorAdjustments)) as ColorAdjustments;

                openedProfileVignette.active = true;
                openedProfileWhiteBalance.active = true;
                openedProfileFilmGrain.active = true;
                openedProfileTonemapping.active = true;
                openedProfileColorAdjustment.active = true;

                openedProfile = profile;

                AssetDatabase.CreateAsset(profile, filePath);
            }
            rect.y += 20;

            EditorGUI.LabelField(rect, "Opened File: " + (openedProfile == null ? "NULL" : openedProfile.name) );
            rect.y += 20;

            if (openedProfile != null)
            {
                previewToggle = EditorGUI.Toggle(rect, "Preview Filter", previewToggle);

                PreviewSwitch(previewToggle);
            }
        }
    }
}

#endif