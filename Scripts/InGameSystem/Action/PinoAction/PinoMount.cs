using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PinoMount : ActionBase
    {
        public override string ActionName { get; protected set; } = "Mount";
        [SerializeField] int damage = 5;

        public override void ActionUpdate()
        {
            return;
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            BossController bossInstance = BossController.Instance;

            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEffectStart:
                    
                    if(BossController.Instance.CurrentPatternMethod == BossController.MountedFlyingPattern)
                    {
                        bossInstance.bossEntity.EntityAnimator.SetTrigger("MountedHit");
                        bossInstance.bossEntity.GetComponent<BossActionMountedFly>().isHit = true;
                        bossInstance.bossEntity.GetDamage(damage);
                        GameObject trigger = GameObjectPoolManager.Instance.CreateGameObject(bossInstance.mountedAttackPrefab, BossController.RidingPoint.position, Quaternion.identity);
                        trigger.GetComponent<DamageEffectLevelEvent>().owner = entity;
                        trigger.SetActive(true);
                        PlayerCamera.Instance.Shake("BasicSwordAttack");
                    }
                    break;
                case AnimationEventTriggerType.AnimationEnd:
                    if(bossInstance.bossEntity.GetComponent<BossActionMountedFly>().isHit == true)
                    {
                        entity.EntityAnimator.ResetTrigger("MountAttack");
                        entity.EntityAnimator.SetTrigger("Mount");
                        PlayerController.Instance.mountedAttackIntervalTimer.GoodToGo = true;
                    }
                    break;
            }
        }

        public override void End()
        {
            entity.EntityData.godModeTimer = 0.0f;
            return;
        }

        public override void Excute()
        {
            entity.EntityData.godModeTimer = float.PositiveInfinity;
            return;
        }

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            if(changeActionType == ActionType.Jump)
            {
                return false;
            }
            return true;
        }
    }
}