using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace Olympus
{
    public class BossActionAttack : ActionBase
    {
        public override string ActionName { get; protected set; } = "AttackFlag";

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            BossController controller = BossController.Instance;

            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationStart:
                    entity.EntityAnimator.ResetTrigger("AttackFlag");
                    controller.IsAttacking = true;
                    controller.IsStatusChangeable = false;

                    break;
                case AnimationEventTriggerType.AnimationEnd:
                    controller.IsStatusChangeable = true;
                    entity.SetActionType(ActionType.Idle);
                    break;
                case AnimationEventTriggerType.AnimationAttack:
                    controller.PlayVFX("Boss_Attack_VFX");
                    SoundManager.Instance.PlaySound("Homeros_Scratch");
                    BossController.PhaseSet phase = BossController.predefinedPhaseSets[controller.Phase];  
                    HitCheck(4.0f);
                    break;
            }
        }

        void HitCheck(float radius)
        {
            RaycastHit hitInfo;
            if (entity.EntitySphereCast(entity.transform.forward, radius, out hitInfo))
            {
                LogUtil.Log("Boss attacked");
                EntityBase playerEntity = PlayerController.Instance.PlayerEntity;
                if (hitInfo.collider.gameObject == playerEntity.gameObject)
                {
                    playerEntity.GetDamage(entity.EntityData.attackDamage);
                }
            }
        }

        
        public override void ActionUpdate()
        {

        }

        public override void End()
        {
            BossController.ResetStates();

        }

        public override void Excute()
        {
        }
    }
}