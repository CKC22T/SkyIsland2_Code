using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

#if UNITY_EDITOR
using UnityEditor;
namespace Olympus
{
    public class TextureViewer : EditorWindow
    {
        InteractionComputeObject targetObject;

        static TextureViewer instance;
        private RenderTexture targetTexture;
        [MenuItem("Window/Interactive System Buffer Viewer")]
        static void CreateWindow()
        {
            instance = GetWindow<TextureViewer>();
        }


        private void Update()
        {

            EditorApplication.CallbackFunction update = null;

            update = () =>
            {
                EditorApplication.update -= update;
                instance.Repaint();
            };

            EditorApplication.update += update;

        }
        private void OnGUI()
        {
            if(instance == null)
            {
                instance = GetWindow<TextureViewer>();
                return;
            }

            targetObject = EditorGUILayout.ObjectField(targetObject, typeof(InteractionComputeObject), true) as InteractionComputeObject;

            if(instance.targetObject  == null)
            {
                return;
            }

            RenderTexture texture = instance.targetObject.VegetationTexture;

            if(texture == null)
            {
                return;
            }

            Rect rect = EditorGUILayout.GetControlRect();
            rect.width = 512;
            rect.height = 512;

            //GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
            EditorGUI.DrawPreviewTexture(rect, texture);

            rect.x += 512;

            EditorGUI.DrawPreviewTexture(rect, instance.targetObject.TrailTexture);
        }
    }
}
#endif