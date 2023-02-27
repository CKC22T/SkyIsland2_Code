using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SwordWeaponAttackAction : WeaponActionBase
    {
        //[SerializeField, TabGroup("Component")] private DamageEffectLevelEvent damageObject;
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent swordAttack;
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent swordAttack2;
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent swordAttack3;

        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent[] swordSkillAttack;
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent[] swordSkillAttack2;

        [SerializeField, TabGroup("Component")] public float upgradeDamage;

        public override string ActionName { get; protected set; } = "SwordAttack";
        public string ActionName1 { get; protected set; } = "SwordAttack";
        public string ActionName2 { get; protected set; } = "SwordAttack2";
        public string ActionName3 { get; protected set; } = "SwordAttack3";

        public float attack3MoveSpeed = 12.0f;

        public override void ActionUpdate()
        {
        }

        public override void ActionFixedUpdate()
        {
            if (isMove)
            {
                entity.EntityData.moveDirection = entity.transform.forward;
                if (weapon.WeaponData.attackCount == 3)
                {
                    entity.EntityMove(0, acceleration, attack3MoveSpeed, false);
                }
                else
                {
                    entity.EntityMove(0, acceleration, moveSpeed, false);
                }
            }
        }

        public override void Excute()
        {
            base.Excute();

            if (weapon.WeaponData.IsOnBuff)
            {
                isBuffAttack = true;

                if (weapon.WeaponData.IsOnDoubleAttack)
                {
                    switch (weapon.WeaponData.attackCount)
                    {
                        case 1:
                            weapon.WeaponData.attackCount = 2;
                            break;
                        case 2:
                            weapon.WeaponData.attackCount = 1;
                            break;
                    }
                }
                else
                {
                    weapon.WeaponData.attackCount = 1;
                }
                //ActionName = ActionName1;
                //isDoubleAttack = false;
                //return;
            }
            else
            {
                isBuffAttack = false;

                if (weapon.WeaponData.IsOnDoubleAttack)
                {
                    switch (weapon.WeaponData.attackCount)
                    {
                        case 1:
                            weapon.WeaponData.attackCount = 2;
                            break;
                        case 2:
                            weapon.WeaponData.attackCount = 3;
                            break;
                        case 3:
                            weapon.WeaponData.attackCount = 1;
                            break;
                    }
                }
                else
                {
                    weapon.WeaponData.attackCount = 1;
                }
            }

            switch (weapon.WeaponData.attackCount)
            {
                case 1:
                    ActionName = ActionName1;
                    //SoundManager.Instance.PlaySound("Weapon_Sword");

                    break;
                case 2:
                    ActionName = ActionName2;
                    //SoundManager.Instance.PlaySound("Weapon_Sword_DoubleAttack");

                    break;
                case 3:
                    ActionName = ActionName3;
                    //SoundManager.Instance.PlaySound("Weapon_Sword_TripleAttack");

                    break;
            }

            //if(weapon.WeaponData.IsOnDoubleAttack)
            //{
            //    ActionName = ActionName2;
            //    isDoubleAttack = true;
            //    isBuffAttack = false;
            //}
            //else
            //{
            //    ActionName = ActionName1;
            //    isDoubleAttack = false;
            //    isBuffAttack = false;
            //}
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
                    if (isBuffAttack)
                    {
                        if (weapon.WeaponData.upgradeCount > 0 && weapon.WeaponData.upgradeCount <= swordSkillAttack.Length)
                        {
                            DamageEffectLevelEvent damageObject = null;
                            switch (weapon.WeaponData.attackCount)
                            {
                                case 1:
                                    damageObject = swordSkillAttack[weapon.WeaponData.upgradeCount - 1];
                                    break;
                                case 2:
                                    damageObject = swordSkillAttack2[weapon.WeaponData.upgradeCount - 1];
                                    break;
                            }
                                    //effect = swordSkillAttack[weapon.WeaponData.upgradeCount - 1]; 
                                    //var damageObj = Instantiate(swordSkillAttack[weapon.WeaponData.upgradeCount - 1], entity.transform.position + Vector3.up, entity.transform.rotation);
                            //var damageObj = GameObjectPoolManager.Instance.CreateGameObject(swordSkillAttack[weapon.WeaponData.upgradeCount - 1], entity.transform.position + Vector3.up, entity.transform.rotation);
                            //damageObj.owner = entity;

                            if(damageObject)
                            {
                                var obj = GameObjectPoolManager.Instance.CreateGameObject(damageObject, entity.transform.position, entity.transform.rotation);
                                obj.owner = entity;
                            }
                        }
                    }
                    else
                    {
                        DamageEffectLevelEvent damageObject = null;

                        switch (weapon.WeaponData.attackCount)
                        {
                            case 1:
                                damageObject = swordAttack;

                                break;
                            case 2:
                                damageObject = swordAttack2;

                                break;
                            case 3:
                                damageObject = swordAttack3;


                                break;
                        }

                        if (damageObject)
                        {
                            var obj = GameObjectPoolManager.Instance.CreateGameObject(damageObject, entity.transform.position, entity.transform.rotation);
                            obj.owner = entity;
                            obj.damage = damageObject.damage + Mathf.FloorToInt(damageObject.damage * upgradeDamage * weapon.WeaponData.upgradeCount);
                            //damageObject?.Play();
                        }
                    }

                    //if (isBuffAttack)
                    //{
                    //    weapon.WeaponData.doubleAttackTimer = nextAttackTime;
                    //}
                    //else
                    //{
                    //    //if (isDoubleAttack)
                    //    //{
                    //    //    weapon.WeaponData.doubleAttackTime = 0.0f;
                    //    //}
                    //    //else
                    //    //{
                    //    //    weapon.WeaponData.doubleAttackTime = 1.5f;
                    //    //}
                    //    weapon.WeaponData.doubleAttackTimer = nextAttackTime;
                    //}
                        weapon.WeaponData.doubleAttackTimer = nextAttackTime;
                    isMove = false;
                    break;

                case AnimationEventTriggerType.AnimationStart:
                    isMove = true;
                    break;
            }
        }
    }
}