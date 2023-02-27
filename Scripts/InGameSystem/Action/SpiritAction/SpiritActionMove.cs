using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SpiritActionMove : ActionBase
    {
        public override string ActionName { get; protected set; } = "Move";

        public float moveTime = 2.0f;
        public float chaseTime = 3.0f;
        public float timer = 0.0f;
        public float attackDistance = 5.0f;

        public int attackPattern = 0;

        public override void ActionUpdate()
        {
            timer += Time.deltaTime;
            if (entity.EntityData.targetEntity == null)
            {
                if (timer > moveTime)
                {
                    entity.TryChangeActionType(ActionType.Idle);
                    return;
                }
                else
                {
                    entity.EntityData.moveDirection = entity.transform.forward;
                    if (Physics.CheckSphere(entity.MoveCheckPosition.position, 0.5f))
                    {
                        entity.EntityMove();
                    }
                    else
                    {
                        entity.EntityData.moveDirection = Vector3.Cross(entity.transform.forward, Vector3.up);
                        entity.EntityMove();
                    }

                    //SoundManager.Instance.PlayInstance("Spirit_Walk", false);
                }
            }
            else
            {
                if (attackDistance > Vector3.Distance(entity.transform.position, entity.EntityData.targetEntity.transform.position))
                {
                    if (attackPattern == 0)
                    {
                        entity.TryChangeActionType(ActionType.Attack);
                    }
                    else
                    {
                        entity.TryChangeActionType(ActionType.SecondaryAttack);
                    }
                    return;
                }

                if (timer > chaseTime)
                {
                    entity.TryChangeActionType(ActionType.Idle);
                    return;
                }

                Vector3 dir = entity.EntityData.targetEntity.transform.position - entity.transform.position;
                entity.EntityData.moveDirection = dir.normalized;
                if (Physics.CheckSphere(entity.MoveCheckPosition.position, 0.5f))
                {
                    entity.EntityMove();
                }
                else
                {
                    entity.TryChangeActionType(ActionType.Idle);
                }
            }

        }

        public override void End()
        {
            SoundManager.Instance.StopSound("Spirit_Walk");
        }

        public override void Excute()
        {
            timer = 0.0f;
            attackPattern = Random.Range(0, 2);

            if (entity.EntityData.targetEntity == null)
            {
                entity.EntityData.moveDirection = -entity.transform.forward;
                entity.EntityRotate(Random.Range(-36000, 36000));
            }

            SoundManager.Instance.PlayInstance("Spirit_Respawn");
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEffectStart:
                    SoundManager.Instance.PlaySound("Spirit_Walk");
                    break;
            }
        }
    }
}