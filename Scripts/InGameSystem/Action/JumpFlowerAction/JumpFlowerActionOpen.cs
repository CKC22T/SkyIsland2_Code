using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class JumpFlowerActionOpen : ActionBase
    {
        public override string ActionName { get; protected set; } = "Open";

        public override void ActionUpdate()
        {
        }

        public override void End()
        {
        }

        public override void Excute()
        {
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    entity.TryChangeActionType(ActionType.Attack);
                    break;
            }
        }
    }
}