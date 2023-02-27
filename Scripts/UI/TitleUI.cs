using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using static UnityEngine.EventSystems.EventTrigger;

namespace Olympus
{
    public class TitleUI : UIBase
    {
        #region Inspector
        [SerializeField] private EventTrigger[] buttons;
        [SerializeField] private RectTransform arrow;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private AnimationCurve groupCurve;
        [SerializeField] private float groupSpeed;
        [SerializeField] private PlayableDirector creditTimeline;
        #endregion
        #region Value
        private int index;
        private bool isClicked;
        #endregion

        #region Event
        private new void Awake()
        {
            base.Awake();
            UpdateArrow();

            for (int i = 0; i < buttons.Length; ++i)
            {
                var entry = new Entry();
                var ind = i;
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((e) => {
                    index = ind;
                    UpdateArrow();
                });
                buttons[i].triggers.Add(entry);
            }

            isClicked = false;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SelectArrow();
            }
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                index = Mathf.Clamp(index - 1, 0, buttons.Length - 1);
                UpdateArrow();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                index = Mathf.Clamp(index + 1, 0, buttons.Length - 1);
                UpdateArrow();
            }
        }

        //버튼 클릭 이벤트
        public void GameStart()
        {
            if (isClicked)
                return;

            isClicked = true;
            SoundManager.Instance.PlaySound("UI_Button");
            BookOpener.Instance.Open(() =>
            {
                TitleScenePresenter.Instance.GameStart();
            });
            //StartCoroutine(GroupCor(() =>
            //{
            //    BookOpener.Instance.Open(() =>
            //    {
            //        TitleScenePresenter.Instance.GameStart();
            //    });
            //}));
        }
        public void GameLoad()
        {
            if (isClicked)
                return;

            isClicked = true;
            SoundManager.Instance.PlaySound("UI_Button");
            TitleScenePresenter.Instance.GameLoad();
        }
        public void ShowSettingUI()
        {
            if (isClicked)
                return;

            isClicked = false;
            SoundManager.Instance.PlaySound("UI_Button");
            UIManager.Show(UIList.SettingUI);
        }
        public void ShowAlbumUI()
        {
            if (isClicked)
                return;

            isClicked = false;
            SoundManager.Instance.PlaySound("UI_Button");
            UIManager.Show(UIList.AlbumUI);
        }
        public void ShowCreditUI()
        {
            if (isClicked)
                return;

            creditTimeline.gameObject.SetActive(true);
            //TODO : 없넹...
        }
        public void GameExit()
        {
            if (isClicked)
                return;

            isClicked = true;
            SoundManager.Instance.PlaySound("UI_Button");
            TitleScenePresenter.Instance.GameExit();
        }
        #endregion
        #region Function
        //Public
        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            group.alpha = 1;
        }
        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        //Private
        private void UpdateArrow()
        {
            arrow.anchoredPosition = new Vector2(arrow.anchoredPosition.x, buttons[index].GetComponent<RectTransform>().anchoredPosition.y);
        }
        private void SelectArrow()
        {
            buttons[index].OnPointerClick(null);
        }
        private IEnumerator GroupCor(Action _onEnd)
        {
            group.interactable = false;

            float timer = 0;
            while(timer < 1.0)
            {
                timer += Time.unscaledDeltaTime * groupSpeed;
                group.alpha = groupCurve.Evaluate(timer);
                yield return null;
            }
            _onEnd();
        }
        #endregion
    }
}