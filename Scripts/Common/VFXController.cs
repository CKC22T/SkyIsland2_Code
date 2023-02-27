using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Olympus
{
    public class VFXController
    {
        public VFXController(ParticleSystem targetSystem, float time)
        {
            target = targetSystem;
            PlaybackTime = time;
        }
        public ParticleSystem target;
        public float PlaybackTime = 0.0f;

        void Check()
        {
            if (target.time >= PlaybackTime)
            {
                target.time = 0.0f;
            }
        }
    }
}