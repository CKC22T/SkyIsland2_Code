using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class JumpFlowerActionCloseIdle : ActionBase
    {
        public override string ActionName { get; protected set; } = "CloseIdle";

        public override void ActionUpdate()
        {
            if(entity.EntityData.targetEntity)
            {
                entity.TryChangeActionType(ActionType.Run);
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