using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BossActionFlyGroundCharging : ActionBase
    {
        public override string ActionName { get; protected set; } = "GroundCharge";

        bool IsStomping = true;
        bool PatternEnded = false;
        float flySpeed = 100.0f;
        BossController controller;
        CharacterController controllerRigid;
        [SerializeField] Transform levelCenter;
        bool rayCastLock = false;
        public override void ActionUpdate()
        {
            if (Physics.Raycast(entity.transform.position, -Vector3.up, 1.3f) == true && rayCastLock == false)
            {
                IsStomping = false;
                controller.bossEntity.UseGravity = true;

                RaycastHit hitInfo;
                if (Physics.SphereCast(entity.transform.position, 12.0f, -Vector3.up, out hitInfo) == true)
                {
                    LogUtil.Log("Boss Stomping sphere cast");
                    var playerController = PlayerController.Instance;
                    if (hitInfo.collider.gameObject == playerController.PlayerEntity.gameObject)
                    {
                        playerController.PlayerEntity.GetDamage(1);
                        var rigid = playerController.GetComponent<CharacterController>();

                        Vector3 knockbackDirection = (transform.position - playerController.PlayerEntity.transform.position).normalized;

                        playerController.IsStunned = true;
                        rigid.detectCollisions = false;

                        playerController.stunnedTimer.Reset(5.0f);
                        playerController.noClipTimer.Reset(5.0f);

                        playerController.PlayerEntity.EntityData.moveDirection = knockbackDirection;
                        playerController.PlayerEntity.EntityMove(0.0f, 100.0f * 10.0f, 100.0f, false);
                        playerController.PlayerEntity.EntityJump(25.0f);
                    }
                }

                controllerRigid.detectCollisions = false;
                controller.PlayVFX("Boss_Stomp_Land_VFX");
                entity.EntityAnimator.SetTrigger("EndStomp");



                rayCastLock = true;
            }

            controller = BossController.Instance;
            if (IsStomping == true)
            {
                controller.PlayVFX("Boss_Stomp_VFX");

                EntityBase target = PlayerController.Instance.PlayerEntity;

                Vector3 centerDirection = (levelCenter.position - transform.position).normalized;
                //Vector3 targetXZDirection = target.EntityData.moveDirection;
                //Vector3 targetPoint = targetXZDirection * 8.0F + target.transform.position;
                //Vector3 chargeDir = (targetPoint - entity.transform.position).normalized;

                controllerRigid.Move(centerDirection * 50.0f * Time.deltaTime);
                Debug.DrawLine(entity.transform.position, entity.transform.position + controller.TargetDirection * 50.0f, Color.green, 1.0f);

            }
            else
            {

            }
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            if (PatternEnded == true)
            {
                return;
            }
            var controller = BossController.Instance;
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationStart:
                    if (IsStomping == true)
                    {
                        //   entity.EntityAnimator.ResetTrigger("GroundCharge");
                        controller.bossEntity.EntityData.moveDirection = -controller.TargetDirection;
                        Debug.DrawLine(entity.transform.position, entity.transform.position + controller.bossEntity.EntityData.moveDirection * controller.TargetDistance, Color.red, 10.0f);

                    }
                    else
                    {
                    }
                    break;

                case AnimationEventTriggerType.AnimationAttack:

                    break;

                case AnimationEventTriggerType.AnimationEnd:
                    if (IsStomping == false)
                    {
                        entity.EntityAnimator.ResetTrigger("EndStomp");
                        SoundManager.Instance.PlaySound("Homeros_SkyDrop");
                        PatternEnded = true;
                        controller.IsStatusChangeable = true;
                        entity.SetActionType(ActionType.Idle);
                        controller.ActionLock = false;
                    }
                    break;
            }
        }

        public override void End()
        {
            controllerRigid.detectCollisions = true;

        }

        public override void Excute()
        {
            controllerRigid = entity.gameObject.GetComponent<CharacterController>();
            IsStomping = true;
            rayCastLock = false;
            PatternEnded = false;
        }
    }
}