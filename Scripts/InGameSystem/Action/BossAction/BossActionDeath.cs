using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BossActionDeath : ActionBase
    {
        [SerializeField] GameObject endingDoor;
        [SerializeField] GameObject timelineObject;
        [SerializeField] GameObject bridgeObject;
        [SerializeField] LevelEventBase timelineEvent;
        public override string ActionName { get; protected set; } = "IsDead";
        public override void ActionUpdate()
        {
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            BossController controller = BossController.Instance;
            switch(eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
               //     entity.EntityAnimator.enabled = false;
                    break;
                case AnimationEventTriggerType.AnimationEffectStart:
                    controller.PlayVFX("Boss_Stomp_Land_VFX");
                    controller.PlayVFX("Boss_Rush_Fly_VFX");
                    SoundManager.Instance.PlaySound("Homeros_Dead");
                    SoundManager.Instance.PlaySound("Homeros_Landing");
                    break;
            }
        }

        public override void End()
        {

        }

        public override void Excute()
        {
            BossController controller = BossController.Instance;
            if(controller.Phase != 2)
            {
                entity.SetActionType(ActionType.Move);
                return;
            }
            UIManager.Hide(UIList.BossHPUI);

            timelineEvent.OnLevelEvent(null);
            endingDoor.SetActive(false);

            BossController.Instance.IsStatusChangeable = false;
        }
    }
}