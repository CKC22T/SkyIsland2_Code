using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class WispIdle : ActionBase
    {
        public override string ActionName { get; protected set; } = "Idle";

        public float idleTime = 1.0f;
        public float timer = 0.0f;

        public float teleportDistance = 3.0f;

        public override void ActionUpdate()
        {
            timer += Time.deltaTime;
            if(timer > idleTime)
            {
                if (entity.EntityData.targetEntity)
                {
                    if (teleportDistance > Vector3.Distance(entity.transform.position, entity.EntityData.targetEntity.transform.position))
                    {
                        entity.TryChangeActionType(ActionType.Teleport);
                    }
                    else
                    {
                        entity.TryChangeActionType(ActionType.Run);
                    }
                }
                else
                {
                    entity.TryChangeActionType(ActionType.Walk);
                    return;
                }
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            timer = 0.0f;
        }
    }
}