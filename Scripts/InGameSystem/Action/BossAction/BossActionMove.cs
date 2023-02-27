using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BossActionMove : ActionBase
    {
        private BossController controller;
        public override string ActionName { get; protected set; } = "IsMoving";

        public override void ActionUpdate()
        {
            Transform targetTransform = PlayerController.Instance.PlayerEntity.transform;

            Vector3 entityToPlayerDirection = (targetTransform.position - transform.position).normalized;
            entity.EntityData.moveDirection = entityToPlayerDirection;
            entity.EntityMove();
            SoundManager.Instance.PlaySound("Homeros_Walk", false);


            if (controller.TargetDistance < 4.0f)
            {
                controller.IsStatusChangeable = true;
                entity.EntityAnimator.SetBool("IsMoving", false);
            }
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch(eventId)
            {
                case AnimationEventTriggerType.AnimationStart:
                    entity.EntityAnimator.SetBool("IsMoving", false);
                    break;
                case AnimationEventTriggerType.AnimationEnd:
                    entity.EntityAnimator.SetBool("IsMoving", true);

                    break;
            }
        }

        public override void End()
        {
            BossController.ResetStates();
            SoundManager.Instance.StopSound("Homeros_Walk");

        }

        public override void Excute()
        {
            controller = BossController.Instance;
            controller.IsStatusChangeable = false;
            entity.EntityAnimator.SetBool("IsMoving", false);
        }
    }
}