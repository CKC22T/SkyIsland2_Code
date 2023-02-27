using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class TargetChangeActionLevelEvent : LevelEventBase
    {
        public EntityBase target;
        public ActionType actionType;

        public bool isTriggerTarget = false;

        public override void OnLevelEvent(EntityBase entity)
        {
            if(isTriggerTarget)
            {
                target = entity;
            }

            target.SetActionType(actionType);
        }
    }
}