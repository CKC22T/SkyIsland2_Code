using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class WispWalk : ActionBase
    {
        public override string ActionName { get; protected set; } = "Walk";

        public float walkTime = 1.0f;
        public float timer = 0.0f;

        public override void ActionUpdate()
        {
            timer += Time.deltaTime;
            if(timer > walkTime)
            {
                entity.TryChangeActionType(ActionType.Idle);
                return;
            }

            entity.EntityData.moveDirection = entity.transform.forward;
            if (Physics.CheckSphere(entity.MoveCheckPosition.position, 0.5f))
            {
                entity.EntityMove();
            }
            else
            {
                entity.EntityData.moveDirection = Vector3.Cross(entity.transform.forward, Vector3.up);
                entity.EntityMove();
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            timer = 0.0f;
            entity.EntityData.moveDirection = -entity.transform.forward;
            entity.EntityRotate(Random.Range(-36000, 36000));
        }
    }
}