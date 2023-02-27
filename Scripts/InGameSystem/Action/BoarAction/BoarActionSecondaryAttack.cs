using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BoarActionSecondaryAttack : ActionBase
    {
        public override string ActionName { get; protected set; } = "Attack2";

        public bool isMove;
        public bool isRotate;

        public DamageEffectLevelEvent damageLevelEvent;
        private BoxCollider damageCollider;
        public ParticleSystem particle;

        public float moveSpeed;
        public float time = 0.1f;
        public AnimationCurve curve;
        private float timer;

        private void Start()
        {
            damageCollider = damageLevelEvent.GetComponent<BoxCollider>();
        }

        public override void ActionUpdate()
        {
        }

        public override void ActionFixedUpdate()
        {
            if (isRotate)
            {
                Vector3 dir = entity.EntityData.targetEntity.transform.position - entity.transform.position;
                dir.y = 0;
                entity.EntityData.moveDirection = dir.normalized;
                entity.EntityRotate(360);
            }

            if (isMove)
            {
                timer += Time.fixedDeltaTime;
                entity.physics.LateralVelocity = entity.transform.forward * moveSpeed * curve.Evaluate(timer / time);
            }
        }

        public override void End()
        {
            damageCollider.enabled = false;
        }

        public override void Excute()
        {
            isMove = false;
            isRotate = true;
            timer = time;

            damageLevelEvent.owner = entity;
            damageLevelEvent.damage = entity.EntityData.attackDamage;
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch(eventId)
            {
                case AnimationEventTriggerType.AnimationEffectStart:
                    SoundManager.Instance.PlayInstance("MagicBore_Attack_02");
                    particle.Play();
                    break;

                case AnimationEventTriggerType.AnimationEnd:
                    entity.TryChangeActionType(ActionType.Idle);
                    break;

                case AnimationEventTriggerType.AnimationLock:
                    isMove = true;
                    timer = 0.0f;
                    damageCollider.enabled = true;
                    //damageTrigger.IsTriggerActive.Enter = true;
                    //damageTrigger.IsTriggerRepeat.Enter = true;
                    break;

                case AnimationEventTriggerType.AnimationUnLock:
                    isMove = false;
                    damageCollider.enabled = false;
                    //damageTrigger.IsTriggerActive.Enter = false;
                    //damageTrigger.IsTriggerRepeat.Enter = false;
                    //particle.Stop();
                    //particle.Clear();
                    break;
            }
        }

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            return !damageCollider.enabled;
        }
    }
}