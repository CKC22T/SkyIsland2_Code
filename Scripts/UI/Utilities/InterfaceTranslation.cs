using Olympus;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Olympus
{
    public sealed class InterfaceTranslation : InterfaceAnimateObject
    {

        [SerializeField] protected Vector3 from, to;
        protected override void OnAction(float tick, float t, float evaluation)
        {
            Vector2 interpolated = Vector2.LerpUnclamped(from, to, evaluation);

            Target.anchoredPosition = interpolated;
        }

#if UNITY_EDITOR
        protected override void Record()
        {
            if (RecordFrom == true)
            {
                if(IsSequenceObject == true)
                {
                    from = localTransform.anchoredPosition;
                }
                else
                {
                    from = Target.anchoredPosition;
                }
            }
            if (RecordTo == true)
            {
                if(IsSequenceObject == true)
                {
                    to = localTransform.anchoredPosition;
                }
                else
                {
                    to = Target.anchoredPosition;
                }
            }

        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Handles.color = Color.green;
            Vector3 dir = (to - from).normalized;
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

            if (from != null)
            {
                Handles.ArrowHandleCap(0, from, rot, 50.0f, EventType.Repaint);
            }

            if (to != null)
            {
                Handles.DrawWireDisc(to, Vector3.forward, 10.0f);
            }

            if (from != null && to != null)
            {
                Handles.DrawDottedLine(from, to, 10.0f);
            }
        }
#endif
    }
}