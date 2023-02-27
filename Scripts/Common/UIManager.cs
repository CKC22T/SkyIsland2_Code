using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
namespace Olympus
{
    public class UIManager : SingletonBase<UIManager>
    {
        private void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            if (PlayerController.Instance.IsControlLocked ||
                PlayerController.Instance.PlayerEntity == null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                bool isPause = GameDataManager.Instance.stageIslandType != IslandType.Ending;
                for (int i = 0; i < Instance.ActiveElements.Count; i++)
                {
                    if (Instance.ActiveElements[i].HaveFlag(UIBase.UIFlag.IMMUTABLE) == false)
                    {
                        Instance.ActiveElements[i].Hide();
                        //uiManager.ActiveElements.RemoveAt(i);
                        isPause = false;
                        break;
                    }
                }

                if (isPause)
                {
                    Show(UIList.PauseUI);
                }
            }
        }

        public class UIDraft
        {
            public UIDraft(UIBase[] list)
            {
                elements = new();
                elements.AddRange(list);

                for (int i = 0; i < list.Length; i++)
                {
                    elements[i].Hide();
                }
                elements.Remove(Instance.GetUI(UIList.LoadingUI));
                elements.Remove(Instance.GetUI(UIList.CinematicBarUI));
            }

            public List<UIBase> elements;
            
            public void Apply()
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    elements[i].Show();
                }

                elements.Clear();
                elements = null;
            }
        };

        public List<UIBase> ActiveElements { get; private set; }
        public List<UIBase> RawElements { get; private set; }
        //  public List<UIBase> SavedElements { get; private set; }
        public Stack<UIDraft> Drafts { get; private set; }

        private Dictionary<UIList, UIBase> panels = new();
        private Dictionary<UIList, UIBase> popups = new();

        [SerializeField] private Transform panelRoot;
        [SerializeField] private Transform popupRoot;

        [Button]
        public static UIBase Hide(UIList ui, UnityAction hideCallback = null)
        {
            UIBase element;

            Instance.GetUI(ui);

            if (Instance.panels.TryGetValue(ui, out element) == false)
            {
                Instance.popups.TryGetValue(ui, out element);
            }

            element?.Hide(hideCallback);

            if(Instance.ActiveElements.Contains(element) == true)
            {
                Instance.ActiveElements.Remove(element);
            }
            return element;
        }

        [Button]
        public static void HideAll()
        {
            foreach(var ui in Instance.panels.Values)
            {
                ui?.Hide();
            }
            foreach (var ui in Instance.popups.Values)
            {
                ui?.Hide();
            }
        }

        public static void SaveCurrentStates()
        {
            UIDraft newDraft = new UIDraft(Instance.ActiveElements.ToArray());
            Instance.Drafts?.Push(newDraft);
        }

        public static void RecoverPreviousStatus()
        {
            while(Instance.Drafts.Count > 0)
            {
                UIDraft draft = Instance.Drafts.Pop();
                draft?.Apply();
            }

            //if(Instance.Drafts.Count == 0)
            //{
            //    return;
            //}
            //UIDraft draft = Instance.Drafts.Pop();
            //draft?.Apply();
        }

        public static void IsolateGroup(string groupId)
        {
            foreach(var i in Instance.ActiveElements)
            {
                if(groupId.Equals(i.GroupID) == false)
                {
                    i.Hide();
                }
            }
        }

        public static void DisplayGroup(string groupId, bool flag)
        {
            foreach (var i in Instance.RawElements)
            {
                if(groupId.Equals(i.GroupID) == false)
                {
                    continue;
                }

                if(flag == true)
                {
                    i.Show();
                }
                else
                {
                    i.Hide();
                }
            }
        }

        [Button]
        public static UIBase Show(UIList ui, UnityAction showCallback = null)
        {
            UIBase element;

            Instance.GetUI(ui);

            if (Instance.panels.TryGetValue(ui, out element) == false)
            {
                Instance.popups.TryGetValue(ui, out element);
            }

            element?.Show(showCallback);

            if(Instance.ActiveElements.Contains(element) == false)
            {
                Instance.ActiveElements.Add(element);
            }

            return element;
        }

        private List<UIList> autoHideExceptList = new List<UIList>()
        {
            UIList.DebugUI
        };

        private const string UI_PATH = "UI/Prefab/";

        public void Initialize()
        {
            ActiveElements = new();
            RawElements = new();
            Drafts = new();

            if (panelRoot == null)
            {
                GameObject panelRootGO = new GameObject("Panel Root");
                panelRoot = panelRootGO.transform;
                panelRoot.parent = transform;
                panelRoot.localPosition = Vector3.zero;
                panelRoot.localRotation = Quaternion.identity;
                panelRoot.localScale = Vector3.one;
            }

            if (popupRoot == null)
            {
                GameObject popupRootGO = new GameObject("Popup Root");
                popupRoot = popupRootGO.transform;
                popupRoot.parent = transform;
                popupRoot.localPosition = Vector3.zero;
                popupRoot.localRotation = Quaternion.identity;
                popupRoot.localScale = Vector3.one;
            }

            for (int index = (int)UIList.SCENE_PANEL + 1; index < (int)UIList.MAX_SCENE_PANEL; ++index)
            {
                panels.Add((UIList)index, null);
            }

            for (int index = (int)UIList.SCENE_POPUP + 1; index < (int)UIList.MAX_SCENE_POPUP; ++index)
            {
                popups.Add((UIList)index, null);
            }
        }

        public bool Contains(UIList uiName)
        {
            if (panels.ContainsKey(uiName) == true)
            {
                return true;
            }

            return false;
        }

        public UIBase GetUI(UIList uiName, bool reload = false)
        {
            if (UIList.SCENE_PANEL < uiName && uiName < UIList.MAX_SCENE_PANEL)
            {
                if (!panels.ContainsKey(uiName))
                {
                    return null;
                }

                if (reload && panels[uiName] != null)
                {
                    Destroy(panels[uiName].gameObject);
                    panels[uiName] = null;
                }

                if (panels[uiName] == null)
                {
                    string path = UI_PATH + uiName.ToString();
                    GameObject loadedUI = Resources.Load<GameObject>(path);
                    if (loadedUI == null)
                    {
                        return null;
                    }

                    panels[uiName] = Instantiate(loadedUI, panelRoot).GetComponent<UIBase>();
                    panels[uiName].Id = uiName;
                    RawElements.Add(panels[uiName]);

                    if (panels[uiName])
                    {
                        panels[uiName].gameObject.SetActive(false);
                    }
                }
                return panels[uiName]; 
            }

            if (UIList.SCENE_POPUP < uiName && uiName < UIList.MAX_SCENE_POPUP)
            {
                if (!popups.ContainsKey(uiName))
                {
                    return null;
                }

                if (reload && popups[uiName] != null)
                {
                    Destroy(popups[uiName].gameObject);
                    popups[uiName] = null;
                }

                if (popups[uiName] == null)
                {
                    string path = UI_PATH + uiName.ToString();
                    GameObject loadedUI = Resources.Load<GameObject>(path);
                    if (loadedUI == null)
                    {
                        return null;
                    }

                    popups[uiName] = Instantiate(loadedUI, popupRoot).GetComponent<UIBase>();
                    panels[uiName].Id = uiName;
                    RawElements.Add(panels[uiName]);

                    if (popups[uiName])
                    {
                        popups[uiName].gameObject.SetActive(false);
                    }
                }
                return popups[uiName];
            }

            return null;
        }

    }
}