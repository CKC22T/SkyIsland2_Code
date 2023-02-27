using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class WorldUIRoot : MonoBehaviour
    {
        private static WorldUIRoot instance;
        public static WorldUIRoot Instance
        {
            get
            {
                if(instance == null)
                {
                    var prefab = Resources.Load<GameObject>("UI/WorldSpace/WorldUIRoot");
                    var uiRoot = Instantiate(prefab).GetComponent<WorldUIRoot>();
                    DontDestroyOnLoad(uiRoot);

                    instance = uiRoot;
                }

                return instance;
            }
        }

        public const float unitPerPixel = 0.01f;

        public GameObject AddUIElement(GameObject prefab)
        {
            if (prefab == null) return null;

            var ui = Instantiate(prefab, transform);
            return ui;
        }

        public T AddUIElement<T>(GameObject prefab) where T : MonoBehaviour
        {
            return AddUIElement(prefab).GetComponent<T>();
        }

        public void Initialize()
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        }
    }
}