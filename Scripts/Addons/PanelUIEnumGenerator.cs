#if UNITY_EDITOR

using UnityEditor;

namespace Olympus
{
    public class PanelUIEnumGenerator : AutoEnumerationGenerator<PanelUIEnumGenerator>
    {
        [MenuItem("Window/Auto-PanelUI Enumeration Generator")]
        static void CreateWindow()
        {
            EditorWindow.GetWindow<PopupUIEnumGenerator>();
        }

        private void OnEnable()
        {
            base.OnEnable();

            PanelUIEnumGenerator inst = (Instance as PanelUIEnumGenerator);

            inst.assetList.Clear();

            inst.target = "Assets\\Olympus\\Resources\\UI\\Prefab";
            inst.trackedSourceFile = "Assets\\Olympus\\Scripts\\Common\\UIList.cs";
            inst.filter = "t:prefab";
            inst.beginAnchorString = "// << UI_SCENE_PANEL";
            inst.endAnchorString = "// >> UI_SCENE_PANEL";

            string[] sceneEnumerators = System.Enum.GetNames(typeof(Olympus.SceneType));

            if (sceneEnumerators.Length > 0)
            {
                inst.assetList.AddRange(sceneEnumerators);
            }
        }
    }

}

#endif