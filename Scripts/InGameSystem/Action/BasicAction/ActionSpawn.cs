using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ActionSpawn : ActionBase
    {
        public override string ActionName { get; protected set; } = "Spawn";

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
            switch(eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    entity.SetActionType(ActionType.Idle);
                    break;
            }
        }

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            return false;
        }
    }
}