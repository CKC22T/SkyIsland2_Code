using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BossActionSkill : ActionBase
    {
        BossController controller;
        public override string ActionName { get; protected set; } = "Charging";
        public override void ActionUpdate()
        {

        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationStart:

                    if (controller.IsChargeStarted == false)
                    {
                        entity.EntityAnimator.ResetTrigger("FinishCharging");
                    }
                    break;

                case AnimationEventTriggerType.AnimationAttack:
                    //    entity.EntityAnimator.SetTrigger("FinishCharging");
                    if (controller.IsChargeStarted == false)
                    {
                        controller.IsChargeStarted = true;

                        controller.PlayVFX("Boss_Rush");
                        controller.PlayVFX("Boss_Rush_L_WIND_VFX");
                        controller.PlayVFX("Boss_Rush_R_WIND_VFX");
                    }
                    break;
                case AnimationEventTriggerType.AnimationEffectStart:
                    controller.PlayVFX("Boss_Rush_Fly_VFX");
                    SoundManager.Instance.PlaySound("Homeros_Rush");

                    break;

                case AnimationEventTriggerType.AnimationEnd:

                    if (controller.IsChargeStarted == true)
                    {
                        entity.EntityAnimator.ResetTrigger("StartCharging");
                    }
                    else
                    {
                        controller.SeekingFlag = true;

                        entity.SetActionType(ActionType.Idle);
                    }

                    break;
            }
        }

        public override void End()
        {
            BossController.ResetStates();
            controller.IsStatusChangeable = true;
        }

        public override void Excute()
        {
            LogUtil.Log("Charging Attack");
            controller = BossController.Instance;

            //  entity.TryChangeActionType(ActionType.Idle);
        }
    }
}