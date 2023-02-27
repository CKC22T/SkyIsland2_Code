using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class HammerWeaponAttackAction : WeaponActionBase
    {
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent damageObject;
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent damageObject2;

        [SerializeField, TabGroup("Component")] private StoneDamageEffectLevelEvent[] hammerSkillAttack;

        [SerializeField, TabGroup("Component")] public float upgradeDamage;

        public override string ActionName { get; protected set; } = "HammerAttack";
        public string ActionName1 { get; protected set; } = "HammerAttack";
        public string ActionName2 { get; protected set; } = "HammerAttack2";

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

        public override void Excute()
        {
            base.Excute();

            if (weapon.WeaponData.IsOnBuff)
            {
                ActionName = ActionName2;
                isBuffAttack = true;
                isDoubleAttack = false;
                return;
            }

            if (weapon.WeaponData.IsOnDoubleAttack)
            {
                ActionName = ActionName2;
                isDoubleAttack = true;
                isBuffAttack = false;
            }
            else
            {
                ActionName = ActionName1;
                isDoubleAttack = false;
                isBuffAttack = false;
            }
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            base.AnimationEventAction(eventId);

            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    entity.SetActionType(ActionType.Idle);
                    break;

                case AnimationEventTriggerType.AnimationStart:

                    if (isBuffAttack)
                    {
                        isMove = false;
                    }
                    else
                    {
                        isMove = true;
                    }
                    isMove = true;
                    //SoundManager.Instance.PlaySound("Weapon_Hammer");

                    break;

                case AnimationEventTriggerType.AnimationAttack:
                    ParticleSystem effect = null;

                    if (isBuffAttack)
                    {
                        if (weapon.WeaponData.upgradeCount > 0 && weapon.WeaponData.upgradeCount <= hammerSkillAttack.Length)
                        {
                            var damageObj = GameObjectPoolManager.Instance.CreateGameObject(hammerSkillAttack[weapon.WeaponData.upgradeCount - 1], entity.transform.position + Vector3.up + entity.transform.forward * 3.0f, entity.transform.rotation);
                            damageObj.owner = entity;
                        }
                    }
                    else
                    {
                        if (isDoubleAttack)
                        {
                            var obj = GameObjectPoolManager.Instance.CreateGameObject(damageObject2, entity.transform.position + Vector3.up, entity.AttackPosition.rotation);
                            obj.owner = entity;
                            obj.damage = damageObject2.damage + Mathf.FloorToInt(damageObject2.damage * upgradeDamage * weapon.WeaponData.upgradeCount);

                        }
                        else
                        {
                            var obj = GameObjectPoolManager.Instance.CreateGameObject(damageObject, entity.transform.position + Vector3.up, entity.AttackPosition.rotation);
                            obj.owner = entity;
                            obj.damage = damageObject.damage + Mathf.FloorToInt(damageObject.damage * upgradeDamage * weapon.WeaponData.upgradeCount);

                        }
                    }
                    effect?.Play();

                    if (isDoubleAttack)
                    {
                        weapon.WeaponData.doubleAttackTimer = 0.0f;
                        //SoundManager.Instance.PlaySound("Weapon_Hammer_DoubleAttack");
                    }
                    else
                    {
                        weapon.WeaponData.doubleAttackTimer = nextAttackTime;
                    }

                    isMove = false;
                    break;
            }
        }
    }
}