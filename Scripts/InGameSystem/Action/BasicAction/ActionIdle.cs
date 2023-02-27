using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ActionIdle : ActionBase
    {
        public override string ActionName { get; protected set; } = "Idle";

        public override void ActionUpdate()
        {
        }
        public override void ActionFixedUpdate()
        {
            //if (0.01f < entity.EntityData.moveDirection.sqrMagnitude)
            //{
            //    entity.TryChangeActionType(ActionType.Move);
            //    return;
            //}
        }

        public override void End()
        {
        }

        public override void Excute()
        {
        }
    }
}