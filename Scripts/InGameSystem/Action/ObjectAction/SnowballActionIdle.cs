using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SnowballActionIdle : ActionBase
    {
        public override string ActionName { get; protected set; } = "Idle";
        int layerMask = 0;
        public bool playerHit = false;
        private bool lockedTarget = false;
        DeltaTimer timer = new(1.5f);
        [SerializeField] AnimationCurve snowballCurve;
        public override void ActionUpdate()
        {
            float tick = timer.Tick();
            float normalizedTick = tick / timer.Target;
            if (timer.IsDone == true)
            {
                if (lockedTarget == false)
                {
                    entity.EntityData.moveDirection = BossController.Instance.TargetDirection;
                    entity.PhysicsApplication = true;
                    lockedTarget = true;
                    SoundManager.Instance.PlayInstance("Homeros_SnowBall_Throw", true);
                }

                entity.EntityMove();
                RaycastHit hitInfo;

                Vector3 origin = entity.transform.position + new Vector3(0, 1, 0);
                Ray playerRay = new Ray(origin, BossController.Instance.TargetDirection);
                Ray groundRay = new Ray(origin, -entity.transform.up);
                Debug.DrawRay(origin, entity.transform.forward, Color.magenta, 1.0f);
                layerMask = 1 << LayerMask.NameToLayer("Player");
                if (Physics.SphereCast(playerRay, 0.5f, out hitInfo, 1.0f, layerMask) == true && entity.UseGravity == true)
                {
                    PlayerController.Instance.PlayerEntity.GetDamage(1);
                    entity.PhysicsApplication = false;
                    playerHit = true;
                    entity.SetActionType(ActionType.Dead);
                }
                LayerMask groundLayer = 1 << LayerMask.NameToLayer("Environment");

                if (Physics.Raycast(groundRay, out hitInfo, 1.0f, groundLayer, QueryTriggerInteraction.Ignore) == true)
                {
                    if (hitInfo.collider.gameObject != BossController.Instance.bossEntity.gameObject)
                    {
                        entity.EntityData.moveDirection = Vector3.zero;
                        entity.EntityData.acceleration = 0.0f;
                        entity.EntityData.moveSpeed = 0.0f;
                        entity.UseGravity = false;
                        LogUtil.Log("Stopper: " + hitInfo.collider);
                    }

                }
            }
            else
            {
                float evaluation = snowballCurve.Evaluate(normalizedTick);
                transform.localScale = new Vector3(evaluation, evaluation, evaluation);
            }
        }

        public override void End()
        {
            timer.Reset();
            lockedTarget = false;
        }

        public void Explode()
        {
            playerHit = true;
            entity.SetActionType(ActionType.Dead);
        }
        public override void Excute()
        {
            playerHit = false;
            entity = GetComponent<EntityBase>();
            entity.PhysicsApplication = false;
            //transform.LookAt(PlayerController.Instance.PlayerEntity.transform);
            entity.EntityWarp(BossController.Instance.bossEntity.transform.position + new Vector3(0.0f, 20.0f, 0.0f));
            if (BossController.Instance.PreviousSnowballs.Contains(entity) == false)
            {
                BossController.Instance.PreviousSnowballs.Add(entity);
            }
        }
    }
}