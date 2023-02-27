using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Olympus
{
    public class InterfaceScale : InterfaceAnimateObject
    {
        [SerializeField] protected Vector3 from, to;
        public bool UseRawScale;
        protected override void OnAction(float tick, float t, float evaluation)
        {
            Vector2 interpolated = Vector2.LerpUnclamped(from, to, evaluation);

            if(UseRawScale == true)
            {
                Target.localScale = interpolated;
            }
            else
            {
                Target.sizeDelta = interpolated;
            }
        }

#if UNITY_EDITOR
        protected override void Record()
        {
            if(RecordFrom == true)
            {
                if(IsSequenceObject == true)
                {
                    if (UseRawScale == true)
                    {
                        from = Target.localScale;
                    }
                    else
                    {
                        from = Target.sizeDelta;
                    }
                }
                else
                {
                    if (UseRawScale == true)
                    {
                        from = localTransform.localScale;
                    }
                    else
                    {
                        from = localTransform.sizeDelta;
                    }
                }

            }
            if(RecordTo == true)
            {
                if (IsSequenceObject == true)
                {
                    if (UseRawScale == true)
                    {
                        to = Target.localScale;
                    }
                    else
                    {
                        to = Target.sizeDelta;
                    }
                }
                else
                {
                    if (UseRawScale == true)
                    {
                        to = localTransform.localScale;
                    }
                    else
                    {
                        to = localTransform.sizeDelta;
                    }
                }
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Handles.color = Color.green;
            Vector3 dir = (to - from).normalized;
            Quaternion up = Quaternion.LookRotation(Vector3.zero, Vector3.up);
            Quaternion right = Quaternion.LookRotation(Vector3.zero, Vector3.right);
           
            Handles.ArrowHandleCap(1, Target.anchoredPosition, up, 50.0f, EventType.Repaint);
            Handles.ArrowHandleCap(1, Target.anchoredPosition, right, 50.0f, EventType.Repaint);

            if (from != null)
            {
                Handles.DrawWireCube(Target.anchoredPosition, from);
            }

            if (to != null)
            {
                Handles.DrawWireCube(Target.anchoredPosition, to);
            }

            if (from != null && to != null)
            {
                Handles.DrawDottedLine(from, to, 10.0f);
            }
        }
#endif
    }
}