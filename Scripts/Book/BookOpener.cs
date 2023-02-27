using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Olympus
{
    public class BookOpener : MonoBehaviour
    {
        [Serializable] private struct LocalizeTexture2D
        {
            public SystemLanguage lang;
            public Texture2D[] tex;
        }

        public static BookOpener Instance { get; private set; }
        #region Inspector
        [TabGroup("Component"), SerializeField] private Animator mAnimator;
        [TabGroup("Component"), SerializeField] private GameObject mTitleCinematic;
        [TabGroup("Component"), SerializeField] private Material mLeftPage;
        [TabGroup("Component"), SerializeField] private Material mRightPage;
        [TabGroup("Component"), SerializeField] private GameObject mLeftObject;
        [TabGroup("Component"), SerializeField] private GameObject mRightObject;
        //[TabGroup("Component"), SerializeField] private GameObject mBookUI;
        [TabGroup("Option"), SerializeField] private float mOpenTime = 1.6f;
        [TabGroup("Option"), SerializeField] private float mTurnTime = 2.4f;
        [TabGroup("Component"), SerializeField] private LocalizeTexture2D[] mPageTex;
        [TabGroup("Component"), SerializeField] private AnimationEventListener animationEventListener;
        #endregion
        #region Get,Set
        /// <summary>
        /// 책이 현재 열려있는 상태인지
        /// </summary>
        public bool IsOpened { get; private set; }
        /// <summary>
        /// 현재 뭔가 애니메이션이 재생중인 상태인지
        /// </summary>
        public bool IsDelay { get => mDelayCor != null; }
        #endregion
        #region Value
        private Action mOnEnd;
        private Coroutine mDelayCor;
        private int mPage;
        private Dictionary<SystemLanguage, Texture2D[]> mPageTexDic = new Dictionary<SystemLanguage, Texture2D[]>();
        #endregion

        #region Event
        private void Awake()
        {
            Instance = this;

            foreach (var v in mPageTex)
                mPageTexDic.Add(v.lang, v.tex);

            mLeftObject.SetActive(false);
            mRightObject.SetActive(false);
            animationEventListener.OnAnimationEventAction = AnimationEventListener;
        }

        public void AnimationEventListener(AnimationEventTriggerType eventId)
        {
            switch(eventId)
            {
                case AnimationEventTriggerType.AnimationEffectStart:
                    SoundManager.Instance.PlaySound("UI_turn(033)");
                    break;
            }
        }

        //private void Start()
        //{
        //    Open(null);
        //}
        private void Update()
        {
            if (IsOpened && Input.anyKeyDown)
            {
                if (IsDelay)
                    Fast();
                else
                    NextPage();
            }
        }

        //Animation Event
        public void OnTurnStart()
        {
            mRightObject.SetActive(true);
        }
        public void OnTurnEnd()
        {
            mLeftObject.SetActive(false);
        }
        #endregion
        #region Function
        //Public
        /// <summary>
        /// 책을 펼칩니다.
        /// </summary>
        /// <param name="_onEnd"></param>
        public void Open(Action _onEnd)
        {
            IsOpened = true;
            mOnEnd = _onEnd;

            //mBookUI.SetActive(true);
            mPage = 0;
            if (mTitleCinematic)
                mTitleCinematic.SetActive(true);
            mLeftPage.SetTexture("_BaseColorTex", mPageTexDic[TextTable.Instance.Current][0]);
            mAnimator.Play("Open");
            mDelayCor = StartCoroutine(DelayCor(mOpenTime));
            ++mPage;
            BookUI.Instance.Open(mPage);
        }
        /// <summary>
        /// 다음 페이지로 넘어간다.
        /// </summary>
        public void NextPage()
        {
            if (IsDelay)
                return; //아직 넘기는중인 경우

            if (mPage == mPageTexDic[TextTable.Instance.Current].Length)
            {
                mOnEnd?.Invoke();
                return; //마지막 페이지인 경우
            }

            Olympus.SoundManager.Instance.PlaySound("UI_turn");
            mLeftObject.SetActive(1 < mPage);
            mRightObject.SetActive(false);
            mLeftPage.SetTexture("_BaseColorTex", mPageTexDic[TextTable.Instance.Current][mPage - 1]);
            mRightPage.SetTexture("_BaseColorTex", mPageTexDic[TextTable.Instance.Current][mPage]);
            mAnimator.Play("Turn", 0, 0);
            mAnimator.speed = 1;
            mDelayCor = StartCoroutine(DelayCor(mTurnTime));
            ++mPage;
            BookUI.Instance.Open(mPage);
        }
        /// <summary>
        /// 더 빠르게 만듭니다.
        /// </summary>
        public void Fast()
        {
            mAnimator.speed += 5.0f;
        }
        /// <summary>
        /// 바로 완료 이벤트를 호출합니다.
        /// </summary>
        public void Skip()
        {
            mOnEnd?.Invoke();
        }

        private IEnumerator DelayCor(float _delay)
        {
            float timer = 0;
            while (true)
            {
                timer += Time.deltaTime * mAnimator.speed;
                yield return null;

                if (_delay <= timer)
                    break;
            }
            mDelayCor = null;
        }
        #endregion
    }
}