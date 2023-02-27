using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PinoWalk : ActionBase
    {
        public override string ActionName { get; protected set; } = "Walk";

        public override void ActionUpdate()
        {
        }

        public override void ActionFixedUpdate()
        {
            if (entity.EntityData.moveDirection.sqrMagnitude < 0.01f)
            {
                entity.EntityIdle();
                if (entity.physics.LateralVelocity.sqrMagnitude < 0.01f)
                    entity.SetActionType(ActionType.Idle);
                //SoundManager.Instance.StopSound("Walk_Dirt");
            }
            else
            {
                //SoundManager.Instance.PlaySound("Walk_Dirt", false);
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
            if (changeActionType == ActionType.Move)
            {
                return false;
            }

            return true;
        }
    }
}