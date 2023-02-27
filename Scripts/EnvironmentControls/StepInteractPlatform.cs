using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class StepInteractPlatform : MonoBehaviour
    {
        public static EntityBase player => PlayerController.Instance.PlayerEntity;

        [SerializeField]
        private Collider collider;

        [SerializeField]
        private Transform Root;

        [SerializeField]
        private float runtime = 0.3f;

        [SerializeField]
        private float rotateAngle = 2;

        private Quaternion defaultRotation;

        private float weight = 0;
        private Vector3 hitPoint;
        private Vector3 Axis;

        private bool EnterState = false;
        private Coroutine stepRoutine = null;
        //private CoroutineWrapper wrapper;

        private void Awake()
        {
            defaultRotation = Root.localRotation;

            //wrapper = new CoroutineWrapper(this);
            //EnterState.OnDataChanged += EnterState_OnDataChanged;
        }

        private void FixedUpdate()
        {
            int count = 0;
            bool isEnter = false;
            Vector3 SumPosition = Vector3.zero;
            //foreach (var player in player)
            //{
            //    if (collider.Raycast(new Ray(player.transform.position + Vector3.up, Vector3.down), out var hitInfo, 2.0f))
            //    {
            //        isEnter = true;
            //        SumPosition += hitInfo.point;

            //        count++;
            //    }
            //}

            if (collider.Raycast(new Ray(player.transform.position + Vector3.up, Vector3.down), out var hitInfo, 2.0f))
            {
                isEnter = true;
                SumPosition += hitInfo.point;

                count++;
            }


            if (isEnter)
            {
                hitPoint = SumPosition / count;

                var dir = hitPoint - Root.position;
                Axis = Vector3.Cross(dir, Vector3.up);
            }

            //EnterState.Value = isEnter;
            EnterState_OnDataChanged(isEnter);
        }

        private void EnterState_OnDataChanged(bool isEnter)
        {
            if (EnterState == isEnter)
                return;

            EnterState = isEnter;

            if(stepRoutine != null)
            {
                StopCoroutine(stepRoutine);
                stepRoutine = null;
            }

            if (isEnter)
            {
                stepRoutine = StartCoroutine(OnEnter());
            }

            if (!isEnter)
            {
                stepRoutine = StartCoroutine(OnExit());
            }
        }

        private IEnumerator OnEnter()
        {
            float t = 0;
            var defaultWeight = weight;
            while (t < runtime)
            {
                weight = Mathf.Lerp(defaultWeight, 1, t / runtime);
                t += Time.deltaTime;
                yield return null;
            }
        }

        private void Update()
        {
            if (!EnterState)
                return;

            var dir = hitPoint - Root.position;
            Axis = Vector3.Cross(dir.normalized, Vector3.up);
            Root.localRotation = defaultRotation * Quaternion.AngleAxis(-dir.magnitude * rotateAngle * weight, Axis.normalized);
        }

        private IEnumerator OnExit()
        {
            float t = 0;
            var defaultWeight = weight;
            var startRot = Root.localRotation;
            while (t < runtime)
            {
                Root.localRotation = Quaternion.SlerpUnclamped(startRot, defaultRotation, t / runtime);
                weight = Mathf.Lerp(defaultWeight, 0, t / runtime);

                t += Time.deltaTime;
                yield return null;
            }

            Root.localRotation = Quaternion.SlerpUnclamped(startRot, defaultRotation, 1);
        }

#if UNITY_EDITOR
        [Sirenix.OdinInspector.Button]
        public void Initialize()
        {
            collider = GetComponent<Collider>();
            Root = transform;
        }
#endif
    }
}