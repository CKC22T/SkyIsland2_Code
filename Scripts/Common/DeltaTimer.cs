using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    [System.Serializable]
    public class DeltaTimer
    {
        public float Current { get; private set; } = 0.0f;
        [SerializeField] float target;
        public float Target {
            get { return target; }
        }
        public bool IsDone { get; private set; } = false;
        public bool GoodToGo { get; set; }
        public bool UseUnscaled { get; set; } = false;

        private DeltaTimer() { GoodToGo = true; Current = 0.0f; }
        public DeltaTimer(float limit)
        {
            Current = 0.0f;
            target = limit;
            GoodToGo = true;
        }

        public void Reset(bool goodtogoFlag = true)
        {
            Current = 0.0f;
            GoodToGo = goodtogoFlag;
            IsDone = false;
        }

        public void Reset(float newTarget, bool goodtogoFlag = true)
        {
            Current = 0.0f;
            GoodToGo = goodtogoFlag;
            target = newTarget;
        }

        public float Tick(float timeScale = 1.0f)
        {
            if(timeScale > 0.0f)
            {
                IsDone = Current == Target;
            }
            else
            {
                IsDone = Current <= 0.0f;
            }

            if(IsDone == true || GoodToGo == false)
            {
                return Current;
            }

            if(UseUnscaled == false)
            {
                Current += Time.deltaTime * timeScale;
            }
            else
            {
                Current += Time.unscaledDeltaTime * timeScale;
            }
            Current = Mathf.Clamp(Current, 0.0f, target);

            return Current;
        }
    }
}