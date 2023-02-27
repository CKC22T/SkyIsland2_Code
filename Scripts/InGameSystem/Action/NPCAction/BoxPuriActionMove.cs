using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BoxPuriActionMove : ActionBase
    {
        public override string ActionName { get; protected set; } = "Run";
        public float idleDistance;

        public override void ActionUpdate()
        {
        }

        public override void ActionFixedUpdate()
        {
            //if(PlayerController.Instance.IsControlLocked)
            //{
            //    return;
            //}

            if (entity.EntitySphereCast(entity.EntityData.moveDirection, 1.0f, out var hit))
            {
                entity.EntityJump();
            }

            if (entity.EntityData.targetEntity)
            {
                float distance = Vector3.Distance(entity.transform.position, entity.EntityData.targetEntity.transform.position);
                entity.EntityData.moveDirection = (entity.transform.position - entity.EntityData.targetEntity.transform.position).normalized;
                if (entity.EntityData.targetEntity.EntityType == EntityType.Puri)
                {
                    entity.EntityData.moveDirection *= -1;
                    if (distance < idleDistance)
                    {
                        entity.EntityData.targetEntity = null;
                    }
                }
                if (distance > 20.0f)
                {
                    entity.EntityWarp(entity.EntityData.targetEntity.transform.position - entity.EntityData.targetEntity.transform.forward * 1.0f + Vector3.up);
                    entity.EntityData.targetEntity = null;
                }
                else
                {
                    entity.EntityRotate();
                    entity.EntityMove();
                }
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
        }
    }
}