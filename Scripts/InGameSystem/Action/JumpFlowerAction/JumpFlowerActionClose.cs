using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class JumpFlowerActionClose : ActionBase
    {
        public override string ActionName { get; protected set; } = "Close";

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
                    entity.TryChangeActionType(ActionType.Idle);
                    break;
            }
        }
    }
}