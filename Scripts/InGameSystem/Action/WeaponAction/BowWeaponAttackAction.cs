using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BowWeaponAttackAction : WeaponActionBase
    {
        [SerializeField, TabGroup("Component")] private ArrowLevelEvent damageObject;
        public override string ActionName { get; protected set; } = "BowAim";
        public string ActionName1 { get; protected set; } = "BowAttack";
        public string ActionName2 { get; protected set; } = "BowAttack2";

        public ParticleSystem castingEffect;
        public ParticleSystem shootEffect;
        public ParticleSystem castringShootEffect;

        public float castingTimer = 0.0f;

        public float maxCastingTime = 3.0f;

        public int normalDamage = 1;
        public int castingDamage = 2;

        public float normalScale = 0.5f;
        public float castingScale = 2.0f;

        private bool shootSound = false;

        public override void ActionUpdate()
        {
            castingTimer += Time.deltaTime;

            if (!Input.GetMouseButton(1))
            {
                entity.EntityAnimator.SetTrigger("BowShot");

                // 두번씩 재생돼서 bool로 막았슴다
                //if(!shootSound)
                //{
                //    SoundManager.Instance.PlaySound("Weapon_Bow_Shoot", false);
                //    shootSound = true;
                //}

                //SoundManager.Instance.StopSound("Weapon_Bow_Pull");
            }
        }

        public override void End()
        {
            base.End();

            entity.Weapon.weaponModel.SetActive(true);
            weapon.weaponModel.SetActive(false);
            entity.EntityAnimator.ResetTrigger("BowShot");
            
            //SoundManager.Instance.StopSound("Weapon_Arrow_Pull");

            castingEffect.Stop();
            castingEffect.Clear();
            shootSound = false;


            //if(castingTimer > 1.0f)
            //{
            //    var obj = Instantiate(damageObject, entity.attackPosition.position, entity.transform.rotation);
            //    obj.transform.position += obj.transform.forward;
            //    obj.owner = entity;
            //    obj.damage = weapon.WeaponData.attackDamage;
            //    obj.moveSpeed = moveSpeed;
            //    shootEffect.Play();
            //}
        }

        public override void Excute()
        {
            base.Excute();

            entity.Weapon.weaponModel.SetActive(false);
            weapon.weaponModel.SetActive(true);
            castingEffect.Play();
            //SoundManager.Instance.PlaySound("Weapon_Arrow_Pull", false);

            //if (weapon.WeaponData.IsOnDoubleAttack)
            //{
            //    ActionName = ActionName2;
            //    isDoubleAttack = true;
            //}
            //else
            //{
            //    ActionName = ActionName1;
            //    isDoubleAttack = false;
            //}

            castingTimer = 0.0f;
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            base.AnimationEventAction(eventId);

            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    entity.TryChangeActionType(ActionType.Idle);

                    break;

                case AnimationEventTriggerType.AnimationAttack:
                    var obj = GameObjectPoolManager.Instance.CreateGameObject(damageObject, entity.AttackPosition.position, entity.transform.rotation);
                    obj.transform.position += obj.transform.forward;
                    obj.owner = entity;
                    obj.damage = weapon.WeaponData.attackDamage;
                    obj.moveSpeed = moveSpeed;


                    float castingPower = Mathf.Clamp01(castingTimer / maxCastingTime);

                    obj.damage = normalDamage + Mathf.CeilToInt(castingDamage * castingPower);
                    obj.transform.localScale = Vector3.one * (normalScale + castingScale * castingPower);

                    if (castingTimer > maxCastingTime * 0.5f)
                    {
                        //obj.damage *= 2;
                        //obj.transform.localScale = Vector3.one * castingScale;
                        castringShootEffect.Play();
                        SoundManager.Instance.PlaySound("Weapon_Arrow_Casting");
                    }
                    else
                    {
                        //obj.transform.localScale = Vector3.one * normalScale;
                        shootEffect.Play();
                        SoundManager.Instance.PlaySound("Weapon_Arrow");
                    }

                    castingEffect.Stop();
                    castingEffect.Clear();

                    //if (weapon.WeaponData.IsOnBuff)
                    //{
                    //    for (int i = 0; i < 2; ++i)
                    //    {
                    //        Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, -10f * (i + 1), 0.0f) + entity.transform.rotation.eulerAngles);
                    //        var obj2 = Instantiate(damageObject, entity.AttackPosition.position, rotation);
                    //        obj2.transform.position += obj2.transform.forward;
                    //        obj2.owner = entity;
                    //        obj2.damage = weapon.WeaponData.attackDamage;
                    //        obj2.moveSpeed = moveSpeed;
                    //    }
                    //    for (int i = 0; i < 2; ++i)
                    //    {
                    //        Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, 10f * (i + 1), 0.0f) + entity.transform.rotation.eulerAngles);
                    //        var obj2 = Instantiate(damageObject, entity.AttackPosition.position, rotation);
                    //        obj2.transform.position += obj2.transform.forward;
                    //        obj2.owner = entity;
                    //        obj2.damage = weapon.WeaponData.attackDamage;
                    //        obj2.moveSpeed = moveSpeed;
                    //    }
                    //}

                    //if (isDoubleAttack)
                    //{
                    //    weapon.WeaponData.doubleAttackTimer = 0.0f;
                    //}
                    //else
                    //{
                    //    weapon.WeaponData.doubleAttackTimer = 2.0f;
                    //}
                    break;
            }
        }
    }
}