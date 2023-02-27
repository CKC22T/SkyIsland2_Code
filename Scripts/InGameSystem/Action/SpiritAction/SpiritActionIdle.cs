using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SpiritActionIdle : ActionBase
    {
        public override string ActionName { get; protected set; } = "Idle";

        public float idleTime = 1.0f;
        public float timer = 0.0f;

        public override void ActionUpdate()
        {
            timer += Time.deltaTime;
            if(timer > idleTime)
            {
                if (entity.EntityData.targetEntity == null)
                {
                    entity.TryChangeActionType(ActionType.Dead);
                }

                entity.TryChangeActionType(ActionType.Move);
                return;
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            timer = 0.0f;
            entity.EntityData.targetEntity = null;
        }
    }
}