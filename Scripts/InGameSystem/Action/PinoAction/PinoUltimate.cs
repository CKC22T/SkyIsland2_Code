using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PinoUltimate : ActionBase
    {
        public override string ActionName { get; protected set; } = "Ultimate";

        public float timer = 0.0f;
        public float attackDelay = 0.1f;

        public GameObject ultimateEffect;
        public DamageEffectLevelEvent damageObj;

        public float rotationSpeed = 1800.0f;

        public override void ActionUpdate()
        {
            timer += Time.deltaTime;
            if(timer > attackDelay)
            {
                var obj = GameObjectPoolManager.Instance.CreateGameObject(damageObj, entity.transform.position + Vector3.up, entity.transform.rotation);
                obj.owner = entity;
                obj.gameObject.layer = LayerMask.NameToLayer(entity.Weapon.WeaponData.WeaponName + "Attack");
                obj.damage = 10 + (GameDataManager.Instance.weaponLevelSword + GameDataManager.Instance.weaponLevelSpear + GameDataManager.Instance.weaponLevelHammer);
                obj.destoryTimer = attackDelay;
                timer -= attackDelay;

                SoundManager.Instance.PlaySound("Skill_Ultimate", false);
            }

            entity.EntityData.ultimateGauge -= Time.deltaTime;
            if(entity.EntityData.ultimateGauge <= 0.0f)
            {
                entity.TryChangeActionType(ActionType.Idle);
                SoundManager.Instance.StopSound("Skill_Ultimate");
            }
        }

        public override void ActionFixedUpdate()
        {
            if(0.1f <= entity.EntityData.moveDirection.sqrMagnitude)
            {
                entity.EntityMove(10.0f, 150.0f, 15.0f, false);
            }
            entity.EntityData.moveDirection = Quaternion.Euler(entity.transform.rotation.eulerAngles + Vector3.up * 120) * Vector3.forward;
            entity.EntityRotate(rotationSpeed);
        }

        public override void End()
        {
            ultimateEffect.SetActive(false);
            SkillUI ui = UIManager.Instance.GetUI(UIList.SkillUI) as SkillUI;
            ui.SetUltimateNormal();
            SoundManager.Instance.StopSound("Skill_Ultimate");

        }

        public override void Excute()
        {
            ultimateEffect.SetActive(true);
            timer = 0.0f;
            SkillUI ui = UIManager.Instance.GetUI(UIList.SkillUI) as SkillUI;
            ui.SetUltimateActive();
        }

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            if(changeActionType == ActionType.Dead ||
                changeActionType == ActionType.Idle)
            {
                return true;
            }

            return false;
        }
    }
}