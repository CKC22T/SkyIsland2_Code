using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class StoneBridgeCurve : StoneBridge
    {
        public AnimationCurve moveCurve;

        public override IEnumerator OnMoving()
        {
            float timer = 0.0f;

            Vector3 startLocalPosition = transform.localPosition;
            Quaternion startLocalRotation = transform.localRotation;
            Vector3 startLocalScale = transform.localScale;

            while(timer < moveSpeed)
            {
                float t = timer / moveSpeed;
                t = moveCurve.Evaluate(t);
                transform.localPosition = Vector3.Lerp(startLocalPosition, destination.localPosition, t);
                transform.localRotation = Quaternion.Slerp(startLocalRotation, destination.localRotation, t);
                transform.localScale = Vector3.Lerp(startLocalScale, destination.localScale, t);
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            transform.localPosition = destination.localPosition;
            transform.localRotation = destination.localRotation;
            transform.localScale = destination.localScale;

            if (isAutoOff)
            {
                yield return new WaitForSeconds(autoOffTime);
                BridgeOff();
            }
        }

        public override IEnumerator OffMoving()
        {
            float timer = 0.0f;

            Vector3 startLocalPosition = transform.localPosition;
            Quaternion startLocalRotation = transform.localRotation;
            Vector3 startLocalScale = transform.localScale;

            while (timer < moveSpeed)
            {
                float t = timer / moveSpeed;
                t = moveCurve.Evaluate(t);
                transform.localPosition = Vector3.Lerp(startLocalPosition, startPoint.localPosition, t);
                transform.localRotation = Quaternion.Slerp(startLocalRotation, startPoint.localRotation, t);
                transform.localScale = Vector3.Lerp(startLocalScale, startPoint.localScale, t);
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            transform.localPosition = startPoint.localPosition;
            transform.localRotation = startPoint.localRotation;
            transform.localScale = startPoint.localScale;
        }
    }
}