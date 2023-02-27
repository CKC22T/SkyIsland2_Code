using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Olympus
{
    public class TitleScenePresenter : MonoBehaviour
    {
        public static TitleScenePresenter Instance { get; private set; }

        public GameObject clearObject;
        public Animator bookAnimator;
        public PlayableDirector closeTimeline;
        public Coroutine bookCloseRoutine = null;
        public GameObject[] clearUIs;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            UIManager.Hide(UIList.TitleUI);
            Instance = null;
        }

        private void Start()
        {
            if(GameDataManager.Instance.isClear)
            {
                clearObject.SetActive(true);

                bookAnimator.Play("Close");
                bookAnimator.speed = 0;
                closeTimeline.gameObject.SetActive(true);

                closeTimeline.time = closeTimeline.duration;
                closeTimeline.Evaluate();

                foreach(var ui in clearUIs)
                {
                    ui.SetActive(true);
                }
            }

            //UIManager.Show(UIList.TitleUI);
            UIManager.Show(UIList.CursorUI);
            BGMController.Instance.ChangeBGM("Title_Background");
        }

        private void Update()
        {
            if(closeTimeline.gameObject.activeSelf)
            {
                if(bookCloseRoutine != null)
                {
                    return;
                }

                if(Input.GetKeyDown(KeyCode.F))
                {
                    bookCloseRoutine = StartCoroutine(closeBook());
                }
            }
        }

        private IEnumerator closeBook()
        {
            bookAnimator.speed = 1;

            float dt = (float)closeTimeline.duration;

            while(dt > 0)
            {
                dt -= Time.deltaTime / (float)closeTimeline.duration;

                closeTimeline.time = Mathf.Max(dt, 0);
                closeTimeline.Evaluate();

                yield return null;
            }

            closeTimeline.gameObject.SetActive(false);
            foreach (var ui in clearUIs)
            {
                ui.SetActive(false);
            }
            bookCloseRoutine = null;
        }

        public void GameStart()
        {
            //UIManager.Hide(UIList.TitleUI);
            GameDataManager.Instance.ResetData();
            Main.Instance.ChangeScene(GameData.IslandTypeToSceneType(GameDataManager.Instance.stageIslandType));
            SoundManager.Instance.PlaySound("UI_InGame(039)");
        }

        public void GameLoad()
        {
            if(GameDataManager.Instance.LoadData() == IslandType.None)
            {
                GameDataManager.Instance.ResetData();
            }
            Main.Instance.ChangeScene(GameData.IslandTypeToSceneType(GameDataManager.Instance.stageIslandType));
            SoundManager.Instance.PlaySound("UI_InGame(039)");
        }

        public void GameExit()
        {
            Application.Quit();
        }
    }
}