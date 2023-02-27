using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class OldPuriActionIdle : ActionBase
    {
        public override string ActionName { get; protected set; } = "Idle";

        public string[] motionNames;
        public int motionPattern;
        public float motionTimer;
        public float motionTime;

        public override void ActionUpdate()
        {
            motionTimer += Time.deltaTime;
            if(motionTime < motionTimer)
            {
                motionTimer = 0.0f;
                if(motionPattern < motionNames.Length)
                {
                    entity.EntityAnimator.SetTrigger(motionNames[motionPattern]);
                }
                motionPattern = Random.Range(0, motionNames.Length);
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            motionTimer = 0.0f;
        }
    }
}