#if UNITY_EDITOR

using UnityEditor;

namespace Olympus
{
    public class PopupUIEnumGenerator : AutoEnumerationGenerator<PopupUIEnumGenerator>
    {
        [MenuItem("Window/Auto-PopupUI Enumeration Generator")]
        static void CreateWindow()
        {
            EditorWindow.GetWindow<PopupUIEnumGenerator>();
        }

        private void OnEnable()
        {
            base.OnEnable();

            PopupUIEnumGenerator inst = (Instance as PopupUIEnumGenerator);

            inst.assetList.Clear();

            inst.target = "Assets\\Olympus\\Resources\\UI\\Prefab";
            inst.trackedSourceFile = "Assets\\Olympus\\Scripts\\Common\\UIList.cs";
            inst.filter = "t:prefab";
            inst.beginAnchorString = "// << UI_SCENE_POPUP";
            inst.endAnchorString = "// >> UI_SCENE_POPUP";

            string[] sceneEnumerators = System.Enum.GetNames(typeof(Olympus.SceneType));

            if (sceneEnumerators.Length > 0)
            {
                inst.assetList.AddRange(sceneEnumerators);
            }
        }
    }
}

#endif