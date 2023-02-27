using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ActionMove : ActionBase
    {
        public override string ActionName { get; protected set; } = "Move";

        public override void ActionUpdate()
        {
        }
        public override void ActionFixedUpdate()
        {
            if (entity.EntityData.moveDirection.sqrMagnitude < 0.01f)
            {
                entity.EntityIdle();
                if (entity.physics.LateralVelocity.sqrMagnitude < 0.01f)
                    entity.TryChangeActionType(ActionType.Idle);
            }
            else
            {
                entity.EntityMove();
            }
            //entity.EntityMove();
        }

        public override void End()
        {
        }

        public override void Excute()
        {
        }

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            if(changeActionType == ActionType.Move)
            {
                return false;
            }

            return true;
        }
    }
}