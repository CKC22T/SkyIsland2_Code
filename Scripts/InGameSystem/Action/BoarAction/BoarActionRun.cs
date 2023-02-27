using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BoarActionRun : ActionBase
    {
        public override string ActionName { get; protected set; } = "Run";

        public float runTime = 3.0f;
        public float timer = 0.0f;
        public float jumpAttackDistance = 12.0f;

        public float rushAttackDistance = 8.0f;

        public override void ActionUpdate()
        {
            if (entity.EntityData.targetEntity == null)
            {
                entity.TryChangeActionType(ActionType.Idle);
                return;
            }

            float distance = Vector3.Distance(entity.transform.position, entity.EntityData.targetEntity.transform.position);
            if(rushAttackDistance > distance)
            {
                entity.TryChangeActionType(ActionType.SecondaryAttack);
                return;
            }
            else if (jumpAttackDistance > distance)
            {
                entity.TryChangeActionType(ActionType.Attack);
                return;
            }

            timer += Time.deltaTime;
            if (timer > runTime)
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