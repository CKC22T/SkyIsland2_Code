using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class FlowerTrapActionIdle : ActionBase
    {
        public override string ActionName { get; protected set; } = "Idle";

        public float idleTime = 1.0f;
        public float timer = 0.0f;

        public int attackPattern = 0;

        public override void ActionUpdate()
        {
            timer += Time.deltaTime;
            if(timer > idleTime)
            {
                if (entity.EntityData.targetEntity)
                {
                    if (attackPattern == 0)
                    {
                        entity.TryChangeActionType(ActionType.Attack);
                    }
                    else
                    {
                        entity.TryChangeActionType(ActionType.SecondaryAttack);
                    }
                    return;
                }
            }

            if(entity.EntityData.targetEntity)
            {
                Vector3 dir = entity.EntityData.targetEntity.transform.position - entity.transform.position;
                dir.y = 0;
                entity.EntityData.moveDirection = dir.normalized;
                entity.EntityRotate(360);
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            timer = 0.0f;
            attackPattern = Random.Range(0, 2);
        }
    }
}