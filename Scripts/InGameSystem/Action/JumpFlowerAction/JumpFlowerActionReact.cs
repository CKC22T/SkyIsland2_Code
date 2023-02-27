using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class JumpFlowerActionReact : ActionBase
    {
        public override string ActionName { get; protected set; } = "React";

        public JumpLevelEvent jumpEvent;

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
                case AnimationEventTriggerType.AnimationEffectStart:
                    jumpEvent.OnLevelEvent(entity.EntityData.targetEntity);
                    break;
            }
        }
    }
}