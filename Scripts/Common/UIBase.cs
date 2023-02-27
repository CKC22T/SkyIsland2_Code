using Olympus;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

namespace Olympus
{
    public class UIBase : MonoBehaviour
    {
        [Flags]
        public enum UIFlag
        {
            IMMUTABLE = 1,
            STATIC = 2,
        }

        [Sirenix.OdinInspector.Title("UI Show/Hide Callback Events")]
        public UnityEvent OnShowCallbacks;
        public UnityEvent OnHideCallbacks;

        public bool IsFullScreen;
        public bool IsCursor;
        public bool IsActive { get { return gameObject.activeInHierarchy; } }

        [field: SerializeField]
        public UIFlag flags { get; private set; }

        public UIList Id;
        [SerializeField] private string groupId;

        public string GroupID { get { return groupId; } }

        public List<InterfaceAnimateObject> animationInterfaces = new();
        public DeltaTimer animationTimer = new(0.0f);
        public Coroutine animationRoutine = null;

        protected void Awake()
        {
            InitAnimationTimer();
        }

        public void InitAnimationTimer()
        {
            float max = 0;
            foreach (var i in animationInterfaces)
            {
                if (i != null)
                {
                    if (max < i.Duration + i.Delay)
                    {
                        max = i.Duration + i.Delay;
                    }
                }
            }

            animationTimer.Reset(max);
        }


#if UNITY_EDITOR
        [Button()]
        private void Preview(bool reversed = false)
        {
            if (reversed)
            {
                animationTimer.Tick(9999999);
            }
            else
            {
                animationTimer.Reset();
            }
            Animate(reversed);
        }
        [Button()]
        private void Refresh()
        {
            RefreshAnimationList();
        }
#endif

        public void Animate(bool reversed = false, Action OnFinish = null)
        {
            foreach (var i in animationInterfaces)
            {
                i?.Play(animationTimer, reversed);
            }

            if(animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
                animationRoutine = null;
            }

            animationRoutine = StartCoroutine(playAnimation(reversed, OnFinish));
        }
        protected IEnumerator playAnimation(bool reversed = false, Action OnFinish = null)
        {
            if (reversed)
            {
                while (animationTimer.Current > 0.0f)
                {
                    animationTimer.Tick(-1);
                    yield return null;
                }
            }
            else
            {
                while (animationTimer.IsDone == false)
                {
                    animationTimer.Tick(1);
                    yield return null;
                }
            }

            animationRoutine = null;
            OnFinish?.Invoke();
        }

        public void RefreshAnimationList()
        {
            animationInterfaces.Clear();
            animationInterfaces.AddRange(GetComponentsInChildren<InterfaceAnimateObject>());
        }

        public bool HaveFlag(UIFlag flag)
        {
            return (flags & (flag)) != 0;
        }

        public virtual void Show(UnityAction callback = null)
        {
            if (UIManager.Instance.ActiveElements.Contains(this) == false)
            {
                UIManager.Instance.ActiveElements.Add(this);
            }
            
            gameObject.SetActive(true);
            Animate(false);
            callback?.Invoke();
            OnShowCallbacks?.Invoke();


        }

        public virtual void Hide(UnityAction callback = null)
        {
            UIManager.Instance.ActiveElements.Remove(this);

            gameObject.SetActive(false);
            callback?.Invoke();
            OnHideCallbacks?.Invoke();
        }

        public int GetCanvasSortingOrder()
        {
            var canvas = transform.GetComponent<Canvas>();
            return canvas.sortingOrder;
        }
        public void SetCanvasSortingOrder(int sortingOrder)
        {
            var canvas = transform.GetComponent<Canvas>();
            canvas.sortingOrder = sortingOrder;
        }
    }
}