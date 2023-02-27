using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ElevatorLevelEvent : LevelEventBase
    {
        [SerializeField] AnimationCurve curve;
        DeltaTimer timer;
        [SerializeField] Vector3 from, to;
        [SerializeField] float duration;
        [SerializeField] Transform target;
        static Coroutine previousCoroutine = null;
        public override void OnLevelEvent(EntityBase entity)
        {
            if(previousCoroutine != null)
            {
                StopCoroutine(previousCoroutine);
            }

            timer = new(duration);
            previousCoroutine = StartCoroutine(ElevateAsync(entity));
        }

        IEnumerator ElevateAsync(EntityBase entity)
        {
            Vector3 start, end;
            start = target.position;
            end = to;
            while(timer.IsDone == false)
            {
                float tick = timer.Tick();
                float normalizedTick = tick / duration;
                float t = curve.Evaluate(normalizedTick);

                Vector3 interpolated = Vector3.Lerp(start, end, t);
                target.position = interpolated;

                yield return null;
            }
            yield return null;
        }

        private void OnDrawGizmos()
        {
            
        }
    }
}