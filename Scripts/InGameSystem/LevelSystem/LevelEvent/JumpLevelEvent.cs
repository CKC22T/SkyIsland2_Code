using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Olympus
{
    public class JumpLevelEvent : LevelEventBase
    {
        public EntityBase jumpFlower;
        public float maximumAltitude = 20.0f;
        public float duration;
        public float errorMargin;
        public Transform destination;
        public Vector2 offsetDistance;
        public AnimationCurve movementCurve;
        public AnimationCurve yCurve;
        private Vector3 destinationPosition;

        public override void OnLevelEvent(EntityBase entity)
        {
            AnimationCurve[] curves = { movementCurve, yCurve, movementCurve };
            destinationPosition = destination.position + new Vector3(offsetDistance.x, 0.0f, offsetDistance.y);
            StartCoroutine(entity.JumpToDestination(entity.transform.position, destinationPosition, duration, maximumAltitude, curves, errorMargin));
            //entity.EntityJump(jumpPower);
            entity.SetActionType(ActionType.Jump);
            entity.PhysicsApplication = false;
            PlayerCamera.Instance.VelocityTracking = false;
            SoundManager.Instance.PlayInstance("Jumpfoothold_use");
            //jumpFlower.TryChangeActionType(ActionType.SecondaryAttack);
        }

        public int sampleCount = 60;

#if UNITY_EDITOR
        [ExecuteInEditMode]
        private void OnValidate()
        {
            if (destination == null)
            {
                LogUtil.LogError("this jump pad hasn't been assigned any destination!");
                EditorGUIUtility.PingObject(this);
                return;
            }
        }

        public void OnDrawGizmos()
        {
            if (destination == null)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(destination.position, 1.0f);

            float yDist = Mathf.Abs(transform.position.y - destination.position.y) + maximumAltitude;
            Handles.DrawWireCube(transform.position, new Vector3(10.0f, 10.0f, 10.0f));

            //Vector3 dir = (destination.position + transform.position) / 2.0f;
            destinationPosition = destination.position + new Vector3(offsetDistance.x, 0.0f, offsetDistance.y);

            List<Vector3> points = new();
            Handles.color = Color.red;
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            Vector3 dir = destinationPosition - transform.position;

            for (int i = 0; i < sampleCount; i++)
            {
                float yEvaluation = yCurve.Evaluate(i / (float)sampleCount);
                float xzEvaluation = movementCurve.Evaluate(i / (float)sampleCount);

                float x = Mathf.Lerp(transform.position.x, destinationPosition.x, xzEvaluation);
                float z = Mathf.Lerp(transform.position.z, destinationPosition.z, xzEvaluation);
                //float y = Mathf.LerpUnclamped(transform.position.y, destinationPosition.y + maximumAltitude, yEvaluation);

                float ty = transform.position.y + (dir * xzEvaluation).y;
                float y = Mathf.LerpUnclamped(ty, ty + maximumAltitude, yEvaluation);

                Vector3 point = new Vector3(x, y, z);
                float distance = Vector3.Distance(point, destinationPosition);
                if (distance < errorMargin)
                {
                    Gizmos.DrawSphere(point, errorMargin);
                    break;
                }

                points.Add(point);
            }

            Handles.DrawAAPolyLine(3.0f, points.ToArray());
            // Handles.DrawWireArc(dir, Vector3.forward, transform.position, 90.0f, yDist, 2.5f);
        }
#endif
    }
}