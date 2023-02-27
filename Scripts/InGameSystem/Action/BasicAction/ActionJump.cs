using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ActionJump : ActionBase
    {
        public float downGroundCheckDepth = 10.0f;
        [SerializeField, ReadOnly, TabGroup("Debug")] protected bool possibleActionChange = true;

        public override string ActionName { get; protected set; } = "Jump";

        public override void Excute()
        {
            possibleActionChange = true;
        }
        public override void ActionUpdate()
        {
        }
        public override void ActionFixedUpdate()
        {
            if (entity.IsGround)
            {
                entity.TryChangeActionType(ActionType.Idle);
            }
            else
            {
                if (0.01f < entity.EntityData.moveDirection.sqrMagnitude)
                {
                    entity.EntityMove(entity.EntityData.turningDrag, entity.EntityData.acceleration * 0.8f, entity.EntityData.moveSpeed);
                }
                else
                {
                    entity.EntityIdle(entity.EntityData.deceleration * 0.2f);
                }
            }
        }


        public override bool TryChangeActionType(ActionType changeActionType)
        {
            if(changeActionType == ActionType.Jump)
            {
                return false;
            }

            if(changeActionType == ActionType.Move ||
                changeActionType == ActionType.Idle)
            {
                return entity.IsGround && possibleActionChange;
            }

            return possibleActionChange;
        }

        public override void End()
        {
        }
    }
}