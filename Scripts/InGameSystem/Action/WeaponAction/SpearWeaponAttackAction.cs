using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SpearWeaponAttackAction : WeaponActionBase
    {
        //[SerializeField, TabGroup("Component")] private DamageEffectLevelEvent damageObject;
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent spearAttack;
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent spearAttack2;

        [SerializeField, TabGroup("Component")] private ArrowLevelEvent[] spearSkillAttack;

        [SerializeField, TabGroup("Component")] public float upgradeDamage;
        [SerializeField, TabGroup("Component")] public float upgradeSpeed;

        public override string ActionName { get; protected set; } = "SpearAttack";
        public string ActionName1 { get; protected set; } = "SpearAttack";
        public string ActionName2 { get; protected set; } = "SpearAttack2";

        public override void ActionUpdate()
        {
        }

        public override void ActionFixedUpdate()
        {
            if(isBuffAttack)
            {
                if (isMove)
                {
                    entity.EntityData.moveDirection = entity.transform.forward;

                    entity.EntityMove(0, acceleration * (upgradeSpeed * weapon.WeaponData.upgradeCount + 1), moveSpeed * (upgradeSpeed * weapon.WeaponData.upgradeCount + 1), false);
                }
            }
            else
            {
                if(isMove)
                {
                    entity.EntityData.moveDirection = entity.transform.forward;

                        entity.EntityMove(0, acceleration, moveSpeed, false);
                }
            }
        }

        public override void Excute()
        {
            base.Excute();

            if (weapon.WeaponData.IsOnBuff)
            {
                ActionName = ActionName1;
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
                    //SoundManager.Instance.PlaySound("Weapon_Spear");

                    isMove = true;
                    break;

                case AnimationEventTriggerType.AnimationAttack:
                    if (isBuffAttack)
                    {
                        Vector3 offsetPosition = entity.AttackPosition.position;

                        switch(weapon.WeaponData.upgradeCount)
                        {
                            case 1:
                            {
                                var damageObj = GameObjectPoolManager.Instance.CreateGameObject(spearSkillAttack[0], offsetPosition, entity.transform.rotation);
                                damageObj.owner = entity;
                            }
                                break;
                            case 2:
                            {
                                Vector3 offset = Vector3.Cross(entity.transform.forward, Vector3.up).normalized * 0.8f;

                                var damageObj = GameObjectPoolManager.Instance.CreateGameObject(spearSkillAttack[1], offsetPosition + offset, entity.transform.rotation);
                                damageObj.owner = entity;
                                var damageObj2 = GameObjectPoolManager.Instance.CreateGameObject(spearSkillAttack[1], offsetPosition - offset, entity.transform.rotation);
                                damageObj2.owner = entity;
                            }
                                break;
                            case 3:
                            {
                                Vector3 offset = Vector3.Cross(entity.transform.forward, Vector3.up).normalized * 2.0f;

                                var damageObj = GameObjectPoolManager.Instance.CreateGameObject(spearSkillAttack[2], offsetPosition + offset, entity.transform.rotation);
                                damageObj.owner = entity;
                                var damageObj2 = GameObjectPoolManager.Instance.CreateGameObject(spearSkillAttack[2], offsetPosition + entity.transform.forward * 3.0f, entity.transform.rotation);
                                damageObj2.owner = entity;
                                var damageObj3 = GameObjectPoolManager.Instance.CreateGameObject(spearSkillAttack[2], offsetPosition - offset, entity.transform.rotation);
                                damageObj3.owner = entity;
                            }
                                break;
                        }
                    }
                    else
                    {
                        if (isDoubleAttack)
                        {
                            var obj = GameObjectPoolManager.Instance.CreateGameObject(spearAttack2, entity.transform.position + Vector3.up * 0.7f + entity.transform.forward * 1.0f, entity.transform.rotation);
                            obj.owner = entity;
                            obj.damage = spearAttack2.damage + Mathf.FloorToInt(spearAttack2.damage * upgradeDamage * weapon.WeaponData.upgradeCount);
                            SoundManager.Instance.PlaySound("Weapon_Spear_DoubleAttack");

                        }
                        else
                        {
                            var obj = GameObjectPoolManager.Instance.CreateGameObject(spearAttack, entity.transform.position + Vector3.up * 0.7f + entity.transform.forward * 1.0f, entity.transform.rotation);
                            obj.owner = entity;
                            obj.damage = spearAttack.damage + Mathf.FloorToInt(spearAttack.damage * upgradeDamage * weapon.WeaponData.upgradeCount);

                        }
                    }

                    if (isBuffAttack)
                    {

                    }
                    else
                    {
                        if (isDoubleAttack)
                        {
                            weapon.WeaponData.doubleAttackTimer = 0.0f;
                        }
                        else
                        {
                            weapon.WeaponData.doubleAttackTimer = nextAttackTime;
                        }
                    }

                    isMove = false;
                    break;
            }
        }
    }
}