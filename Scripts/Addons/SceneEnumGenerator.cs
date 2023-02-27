using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR

using UnityEditor;

namespace Olympus
{
    public class SceneEnumGenerator : AutoEnumerationGenerator<SceneEnumGenerator>
    {
        [MenuItem("Window/Auto-Scene Enumeration Generator")]
        static void CreateWindow()
        {
            EditorWindow.GetWindow<SceneEnumGenerator>();
        }

        private void OnEnable()
        {
            base.OnEnable();

            SceneEnumGenerator inst = (Instance as SceneEnumGenerator);

            inst.assetList.Clear();

            inst.target = "Assets\\Olympus\\Scenes";
            inst.trackedSourceFile = "Assets\\Olympus\\Scripts\\Common\\Constant.cs";
            inst.filter = "t:scene";
            inst.beginAnchorString = "// << Scene";
            inst.endAnchorString = "// >> Scene";

            string[] sceneEnumerators = System.Enum.GetNames(typeof(Olympus.SceneType));

            if (sceneEnumerators.Length > 0)
            {
                inst.assetList.AddRange(sceneEnumerators);
            }
        }
    }
}
#endif