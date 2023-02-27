using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Olympus
{
    public class PinoDead : ActionBase
    {
        public override string ActionName { get; protected set; } = "Dead";

        public TimelineLevelEvent deadTimelineLevelEvent;

        public override void ActionUpdate()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(deadTimelineLevelEvent.timeline.state == PlayState.Paused)
                {
                    entity.SetActionType(ActionType.Idle);
                    deadTimelineLevelEvent.TimelineEnd(null);

                    entity.GetHeal(entity.EntityData.maxHealth);
                    entity.EntityWarp(GameManager.Instance.CheckPoint);
                    SoundManager.Instance.PlaySound("Player_Respawn", false);
                }
            }
        }

        public override void End()
        {
            //Camera Following;
            PlayerCamera.Instance.trackingTarget = entity.transform;
            //entity.EntityData.health = entity.EntityData.maxHealth;
        }

        public override void Excute()
        {
            //Camera UnFollowing;
            PlayerCamera.Instance.trackingTarget = null;
            entity.EntityData.godModeTimer = 10.0f;
            entity.EntityData.health = 0;
            SoundManager.Instance.PlaySound("Player_Dead",false);

            deadTimelineLevelEvent?.OnLevelEvent(null);
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch(eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:

                    //entity.SetActionType(ActionType.Idle);
                    break;
            }
        }

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            return false;
        }
    }
}