using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PinoForceMove : ActionBase
    {
        public override string ActionName { get; protected set; } = "Move";

        public override void ActionUpdate()
        {
            if (entity.EntityData.movePosition)
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
                }
                else
                {
                    entity.EntityWarp(entity.EntityData.movePosition.position);
                    entity.EntityData.moveDirection = entity.EntityData.movePosition.forward;
                    entity.EntityRotate(5400);
                    entity.EntityData.movePosition = null;
                    entity.SetActionType(ActionType.Idle);
                }
            }
        }

        public override void End()
        {
            entity.EntityData.movePosition = null;
            PlayerController.Instance.InputUnLock(LockType.ForceMove);
        }

        public override void Excute()
        {
            PlayerController.Instance.InputLock(LockType.ForceMove);
        }

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            return false;
        }
    }
}