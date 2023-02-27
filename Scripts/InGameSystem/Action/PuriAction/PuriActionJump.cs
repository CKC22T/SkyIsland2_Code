using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PuriActionJump : ActionBase
    {
        public override string ActionName { get; protected set; } = "Run";
        [SerializeField, TabGroup("Component")] private float idleDistance = 1.0f;

        public override void ActionUpdate()
        {
            if (entity.EntityData.targetEntity)
            {
                float targetDistance = Vector3.Distance(entity.transform.position, entity.EntityData.targetEntity.transform.position);
                if (targetDistance < idleDistance)
                {
                    entity.SetActionType(ActionType.Idle);
                    return;
                }
                else
                {
                    entity.EntityMove(entity.EntityData.turningDrag, entity.EntityData.acceleration * 0.8f, entity.EntityData.moveSpeed);
                }
            }

            if (entity.EntityData.movePosition)
            {
                float targetDistance = Vector3.Distance(entity.transform.position, entity.EntityData.movePosition.position);
                if (targetDistance < idleDistance)
                {
                    entity.SetActionType(ActionType.Idle);
                    return;
                }
                else
                {
                    entity.EntityMove(entity.EntityData.turningDrag, entity.EntityData.acceleration * 0.8f, entity.EntityData.moveSpeed);
                }
            }


            if (entity.IsGround)
            {
                entity.TryChangeActionType(ActionType.Idle);
                return;
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
        }
    }
}