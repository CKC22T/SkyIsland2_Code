using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SwordWeaponDoubleAttackAction : WeaponActionBase
    {
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent damageObject;
        [SerializeField, TabGroup("Component")] private ParticleSystem swordAttack2;


        public override string ActionName { get; protected set; } = "SwordAttack2";

        public override void ActionUpdate()
        {
        }

        public override void ActionFixedUpdate()
        {
            if (isMove)
            {
                entity.EntityData.moveDirection = entity.transform.forward;
                entity.EntityMove(0, acceleration, moveSpeed, false);
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            base.AnimationEventAction(eventId);

            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    entity.SetActionType(ActionType.Idle);
                    break;

                case AnimationEventTriggerType.AnimationAttack:
                    ParticleSystem effect = swordAttack2;
                    var obj = Instantiate(damageObject, entity.AttackPosition.position, entity.AttackPosition.rotation);
                    obj.owner = entity;
                    obj.damage = weapon.WeaponData.attackDamage;

                    if (weapon.WeaponData.IsOnBuff)
                    {
                        obj.transform.localScale *= 2.0f;
                        effect.transform.localScale = Vector3.one * 2.0f;
                    }
                    else
                    {
                        effect.transform.localScale = Vector3.one * 1.0f;
                    }
                    effect.Play();
                    isMove = false;
                    break;

                case AnimationEventTriggerType.AnimationStart:
                    //SoundManager.Instance.PlaySound("Weapon_Sword_DoubleAttack");
                    isMove = true;
                    break;
            }
        }
    }
}