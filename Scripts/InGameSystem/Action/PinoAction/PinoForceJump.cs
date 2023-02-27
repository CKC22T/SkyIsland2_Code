using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PinoForceJump : ActionBase
    {
        public override string ActionName { get; protected set; } = "ForceJump";

        public ParticleSystem jumpParticle;
        public Vector3 jumpEffectOffset;

        public override void ActionUpdate()
        {
        }

        public override void ActionFixedUpdate()
        {
            if (!entity.UseGravity)
            {
                return;
            }

            if (entity.IsGround)
            {
                if (entity.UseGravity)
                {
                    entity.EntityAnimator.SetTrigger("JumpLand");
                }
            }
            else
            {
                entity.EntityAnimator.ResetTrigger("JumpLand");

                //if (0.01f < entity.EntityData.moveDirection.sqrMagnitude)
                //{
                //    entity.EntityMove(entity.EntityData.turningDrag, entity.EntityData.acceleration * 0.8f, entity.EntityData.moveSpeed);
                //}
                //else
                //{
                //    entity.EntityIdle(entity.EntityData.deceleration * 0.2f);
                //}
            }
        }

        public override void End()
        {
            SoundManager.Instance.StopSound("Player_ForceJump");
            entity.EntityAnimator.ResetTrigger("JumpLand");
            entity.EntityAnimator.ResetTrigger("JumpFalling");
        }

        public override void Excute()
        {
            GameObjectPoolManager.Instance.Release(GameObjectPoolManager.Instance.CreateGameObject(jumpParticle.gameObject, entity.transform.position + jumpEffectOffset, Quaternion.Euler(entity.transform.rotation.eulerAngles + Vector3.left * 90.0f)), 0.5f);

            if (entity.Weapon && !string.IsNullOrEmpty(entity.Weapon.WeaponData.WeaponName))
            {
                ActionName = entity.Weapon.WeaponData.WeaponName + "ForceJump";
            }
            else
            {
                ActionName = "SwordForceJump";
            }

            if (entity.Velocity.y < 0.0f)
            {
                entity.EntityAnimator.SetTrigger("JumpFalling");
            }
            entity.EntityAnimator.ResetTrigger("JumpLand");
            entity.EntityJump();
            SoundManager.Instance.PlaySound("Player_ForceJump");
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    entity.SetActionType(ActionType.Idle);
                    break;
                case AnimationEventTriggerType.AnimationEffectStart:
                    SoundManager.Instance.PlaySound("Player_Landing");
                    break;
            }
        }

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            return false;
        }
    }
}