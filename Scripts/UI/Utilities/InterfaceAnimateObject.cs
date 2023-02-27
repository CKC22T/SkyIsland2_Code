using Olympus;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using UnityEditor;
using UnityEngine;

namespace Olympus
{

    [ExecuteAlways()]
    public abstract class InterfaceAnimateObject : MonoBehaviour
    {
        bool undoFlag = false;
        protected UIBase root;
        public RectTransform Target;
        [SerializeField, ReadOnly] protected RectTransform localTransform;

        public AnimationCurve Curve;

        // 외부 값쓰기 방지, Inspector 수정 허용
        //protected DeltaTimer internalTimer = null;
        //protected DeltaTimer internalDelayTimer = null;
        //public DeltaTimer Timer {
        //    get 
        //    {
        //        if(internalTimer == null)
        //        {
        //            internalTimer = new DeltaTimer(duration);
        //            internalTimer.UseUnscaled = true;
        //        }
        //        return internalTimer; 
        //    } 
        //}

        //public DeltaTimer Delay {
        //    get 
        //    { 
        //        if(internalDelayTimer == null)
        //        {
        //            internalDelayTimer = new DeltaTimer(delay);
        //            internalDelayTimer.UseUnscaled = true;
        //        }
        //        return internalDelayTimer; 
        //    } 
        //}

        [SerializeField] private float duration;
        public float Duration {
            get { return duration; }
        }
        [SerializeField] private float delay;
        public float Delay {
            get { return delay; }
        }

        KeyCode previewKeyCode = KeyCode.Alpha5;

        [DisableIf("@RecordTo == true"), ToggleLeft(), HorizontalGroup()]
        public bool RecordFrom;
        [DisableIf("@RecordFrom == true"), ToggleLeft(), HorizontalGroup()]
        public bool RecordTo;

#if UNITY_EDITOR
        protected EditorCoroutine editorAnimatedCoroutine;
#endif
        protected Coroutine animatedCoroutine;

        private Vector2 undoPosition;
        private Vector2 undoScale;
        private Vector3 undoRation;

        static bool sequenceOperationFlag = false;
        [SerializeField, LabelText("시퀀스 객체")] protected bool IsSequenceObject = false;

       // [Button()]
        //private void DelayAdd(float amount)
        //{
        //    Delay.Reset(Delay.Target - amount);
        //    delay -= amount;
        //}
        //[Button()]
        //private void DelayMultiply(float amount)
        //{
        //    Delay.Reset(Delay.Target * amount);
        //    delay *= amount;
        //}

        //[Button()]
        //private void DelayDivide(float amount)
        //{
        //    Delay.Reset(Delay.Target / amount);
        //    delay /= amount;
        //}

        //[Button()]
        //private void DurationMultiply(float amount)
        //{
        //    Timer.Reset(Timer.Target * amount);
        //    duration *= amount;
        //}
        //[Button()]
        //private void DurationAdd(float amount)
        //{
        //    Timer.Reset(Timer.Target + amount);
        //    duration += amount;
        //}
        //[Button()]
        //private void DurationDivide(float amount)
        //{
        //    Timer.Reset(Timer.Target / amount);
        //    duration /= amount;
        //}


        private void Awake()
        {
            ResetTarget();
            if (Register() == false)
            {
                LogUtil.LogError("Animation interface registration failed");
            }
            Init();
        }

        protected virtual void Init()
        {
        }

        public virtual void Animate(DeltaTimer timer)
        {
            Init();
#if UNITY_EDITOR
            editorAnimatedCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(AnimateAsync(timer));
#else
            animatedCoroutine = StartCoroutine(AnimateAsync(timer));
#endif
        }

        public virtual void AnimateReverse(DeltaTimer timer)
        {
            Init();
#if UNITY_EDITOR
            editorAnimatedCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(AnimateReverseAsync(timer));
#else
            animatedCoroutine = StartCoroutine(AnimateReverseAsync(timer));
#endif
        }

        [Button]
        protected void ResetTarget()
        {
            if (localTransform == null)
            {
                localTransform = GetComponent<RectTransform>();
            }
            if (Target == null)
            {
                Target = GetComponent<RectTransform>();
                if (TryGetComponent<UIBase>(out root) == false)
                {
                    root = GetComponentInParent<UIBase>();
                    if (root == null)
                    {
                      //  LogUtil.LogError(typeof(InterfaceAnimateObject).Name + " requires UIBase in the prefab.");
                    }
                }
           //     LogUtil.Assert(Target != null);
            //    LogUtil.Assert(root != null);
                enabled = false;
            }
            else if(root == null)
            {
                if (TryGetComponent<UIBase>(out root) == false)
                {
                    root = GetComponentInParent<UIBase>();
                    if (root == null)
                    {
                        //  LogUtil.LogError(typeof(InterfaceAnimateObject).Name + " requires UIBase in the prefab.");
                    }
                }
            }
        }

        protected virtual void OnFinish()
        {
        //    Target.anchoredPosition = undoPosition;
       //     Target.localScale = undoScale;
       //     Target.rotation = Quaternion.Euler(undoRation);
        }

        public void SetMoment(float moment)
        {
            OnAction(moment, moment, moment);
        }

        private IEnumerator AnimateAsync(DeltaTimer timer)
        {
            yield return new WaitForSeconds(delay);

            while ((timer.Current - delay) < duration)
            {
                float t = (timer.Current - delay) / duration;
                float evaluation = Curve.Evaluate(t);

                OnAction((timer.Current - delay), t, evaluation);

#if UNITY_EDITOR
                SceneView.RepaintAll();
                EditorApplication.RepaintHierarchyWindow();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
                yield return null;
            }
            OnAction(duration, 1, Curve.Evaluate(1));
            OnFinish();

            undoFlag = true;

#if UNITY_EDITOR
            editorAnimatedCoroutine = null;
#endif
            animatedCoroutine = null;

            yield return null;
        }

        private IEnumerator AnimateReverseAsync(DeltaTimer timer)
        {
            while ((timer.Current - delay) > 0.0f)
            {
                float t = (timer.Current - delay) / duration;
                float evaluation = Curve.Evaluate(Mathf.Clamp01(t));

                OnAction((timer.Current - delay), t, evaluation);

#if UNITY_EDITOR
                SceneView.RepaintAll();
                EditorApplication.RepaintHierarchyWindow();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
                yield return null;
            }

            OnAction(0, 0, Curve.Evaluate(0));
            yield return new WaitForSeconds(delay);

            OnFinish();

            undoFlag = true;

#if UNITY_EDITOR
            editorAnimatedCoroutine = null;
#endif
            animatedCoroutine = null;

            yield return null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SceneView.duringSceneGui += OnListenInput;

        }

        private void Update()
        {
            OnListenInput(SceneView.currentDrawingSceneView);
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnListenInput;
        }

        protected abstract void Record();

        private void OnListenInput(SceneView sceneView)
        {
            UnityEngine.Event e = UnityEngine.Event.current;

            if (e == null)
            {
                return;
            }

            switch (e.type)
            {
                case EventType.KeyDown:
                    if (UnityEngine.Event.current.keyCode == KeyCode.F6)
                    {
                        Play(root.animationTimer);
                    }
                    break;
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if (Target == null)
            {
                ResetTarget();
            }

            Record();
        }
#endif

        protected abstract void OnAction(float tick, float t, float evaluation);

        [Button()]
        public void Play(DeltaTimer timer, bool reversed = false)
        {
            RecordFrom = false;
            RecordTo = false;

#if UNITY_EDITOR
            if (editorAnimatedCoroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(editorAnimatedCoroutine);
            }
            else
            {
                undoPosition = Target.anchoredPosition;
                undoScale = Target.localScale;
                undoRation = Target.rotation.eulerAngles;
            }
#else
            if(animatedCoroutine != null)
            {
                StopCoroutine(animatedCoroutine);
            }
            else
            {
                undoPosition = Target.anchoredPosition;
                undoScale = Target.localScale;
                undoRation = Target.rotation.eulerAngles;
            }
#endif

            if (gameObject.activeInHierarchy == false)
            {
                LogUtil.LogWarning("Target interface is currently inactive.");
            }

            if (Target == null)
            {
                ResetTarget();
            }

            if(reversed == true)
            {
                AnimateReverse(timer);
            }
            else
            {
                Animate(timer);
            }
        }

        private bool Register()
        {
            if (root == null)
            {
                LogUtil.LogError("Root is empty.");
                return false;
            }

            //if (root.animationInterfaces.Contains(this) == false)
            //{
            //    root.animationInterfaces.Add(GetComponent<InterfaceAnimateObject>());
            //}

            return true;
        }

    }
}