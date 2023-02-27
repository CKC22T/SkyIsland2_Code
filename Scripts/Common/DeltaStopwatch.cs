using Olympus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class DeltaStopwatch
    {
        public DeltaStopwatch(int labCount = 1)
        {
            labs = new float[labCount];
        }
        private float tick = 0.0f;
        private float[] labs;
        private int labCount = 0;

        public float GetLatestLab()
        {
            return labs[labCount];
        }
        public void Reset()
        {
            tick = 0.0f;
            labCount = 0;
            for (int i = 0; i < labCount; i++)
            {
                labs[i] = 0.0f;
            }
        }

        public float GetLatestTick()
        {
            return tick;
        }

        public void Tick()
        {
            tick = Mathf.Clamp(tick, tick + Time.deltaTime, float.MaxValue);
        }

        public float Lab()
        {
            if (labCount >= labs.Length)
            {
                LogUtil.LogError("Maximum lab count has been reached. DeltaStopwatch::Lab() call will be ignored.");
                return -1.0f;
            }

            labs[labCount++] = tick;

            return tick;
        }


    }
}