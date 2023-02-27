using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class NPCActionMove : ActionBase
    {
        public override string ActionName { get; protected set; } = "Walk";
        [SerializeField, TabGroup("Component")] private float idleDistance = 3.0f;

        public float walkTime = 1.0f;
        public float timer;

        public override void ActionUpdate()
        {
        }

        public override void ActionFixedUpdate()
        {
            if(entity.EntityData.targetEntity)
            {
                float targetDistance = Vector3.Distance(entity.transform.position, entity.EntityData.targetEntity.transform.position);
                if (targetDistance < idleDistance)
                {
                    entity.TryChangeActionType(ActionType.Idle);
                    return;
                }
                Vector3 dir = entity.EntityData.targetEntity.transform.position - entity.transform.position;
                dir.y = 0.0f;
                dir.Normalize();
                entity.EntityData.moveDirection = dir;
                entity.EntityMove();

                if (entity.EntitySphereCast(entity.EntityData.moveDirection, 1.0f, out var hit))
                {
                    entity.EntityJump();
                }
            }
            else if (entity.EntityData.movePosition == null)
            {
                timer += Time.deltaTime;
                if (timer > walkTime)
                {
                    entity.TryChangeActionType(ActionType.Idle);
                    return;
                }

                entity.EntityData.moveDirection = entity.transform.forward;
                entity.EntityMove();
                //if (Physics.CheckSphere(entity.MoveCheckPosition.position, 0.5f))
                //{
                //    entity.EntityMove();
                //}
                //else
                //{
                //    entity.EntityData.moveDirection = Vector3.Cross(entity.transform.forward, Vector3.up);
                //    entity.EntityMove();
                //}
            }
            else
            {
                Vector3 entityPos = entity.transform.position;
                Vector3 targetPos = entity.EntityData.movePosition.position;

                entityPos.y = 0;
                targetPos.y = 0;

                float distance = Vector3.Distance(entityPos, targetPos);

                if (distance > entity.EntityData.moveSpeed * Time.fixedDeltaTime)
                {
                    Vector3 dir = entity.EntityData.movePosition.position - entity.transform.position;
                    dir.y = 0.0f;
                    dir.Normalize();
                    entity.EntityData.moveDirection = dir;
                    entity.EntityMove();

                    if (entity.EntitySphereCast(entity.EntityData.moveDirection, 1.0f, out var hit))
                    {
                        entity.EntityJump();
                    }
                }
                else
                {
                    entity.EntityWarp(entity.EntityData.movePosition.position);
                    entity.EntityData.moveDirection = entity.EntityData.movePosition.forward;
                    entity.EntityRotate(5400);
                    entity.EntityData.movePosition = null;
                    entity.TryChangeActionType(ActionType.Idle);
                }
            }
        }

        public override void End()
        {
            entity.physics.Velocity = Vector3.zero;
        }

        public override void Excute()
        {
        }
    }
}