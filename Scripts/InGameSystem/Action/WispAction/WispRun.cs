using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class WispRun : ActionBase
    {
        public override string ActionName { get; protected set; } = "Run";

        public float runTime = 3.0f;
        public float timer = 0.0f;
        public float attackDistance = 5.0f;

        public override void ActionUpdate()
        {
            if (entity.EntityData.targetEntity == null)
            {
                entity.TryChangeActionType(ActionType.Idle);
                return;
            }
            if (attackDistance > Vector3.Distance(entity.transform.position, entity.EntityData.targetEntity.transform.position))
            {
                entity.TryChangeActionType(ActionType.Attack);
                return;
            }

            timer += Time.deltaTime;
            if(timer > runTime)
            {
                entity.TryChangeActionType(ActionType.Idle);
                return;
            }

            Vector3 dir = entity.EntityData.targetEntity.transform.position - entity.transform.position;
            entity.EntityData.moveDirection = dir.normalized;
            if (Physics.CheckSphere(entity.MoveCheckPosition.position, 0.5f))
            {
                entity.EntityMove();
            }
            else
            {
                entity.TryChangeActionType(ActionType.Idle);
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