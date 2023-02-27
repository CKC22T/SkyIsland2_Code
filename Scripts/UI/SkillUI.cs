using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Olympus
{
    public class SkillUI : UIBase
    {
        public GameObject[] skillOnImage;
        public GameObject[] skillOffImage;

        public Image ultimateGaugeImage;

        public Animator ultimateGuageAnimator;
        public Animator[] weaponBuffAnimators;
        public Animator[] weaponSelectAnimators;
        public Animator selectWeaponAnimator;

        public EntityBase targetEntity;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            targetEntity = PlayerController.Instance.PlayerEntity;
            ChangeWeapon(0);
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        public void ChangeWeapon(int weaponCode)
        {
            if(weaponCode >= 0 && weaponCode < weaponSelectAnimators.Length)
            {
                selectWeaponAnimator.ResetTrigger("Select");
                selectWeaponAnimator.SetTrigger("Deselect");
                selectWeaponAnimator = weaponSelectAnimators[weaponCode];
                selectWeaponAnimator.ResetTrigger("Deselect");
                selectWeaponAnimator.SetTrigger("Select");
            }
        }

        public void Update()
        {
            if(targetEntity == null || targetEntity.Weapon == null)
            {
                return;
            }

            updateSkillCoolTime();
            updateUltimateGauge();
        }

        private void updateSkillCoolTime()
        {
            for(int i = 0; i < targetEntity.WeaponList.Count && i < weaponBuffAnimators.Length; ++i)
            {
                WeaponBase weapon = targetEntity.WeaponList[i];
                bool isOnBuff = weapon.WeaponData.IsOnBuff && weapon == targetEntity.Weapon;
                weaponBuffAnimators[i].SetBool("On", isOnBuff);
                //if(skillOnImage[i].activeInHierarchy != isOnBuff)
                //{
                //    weaponBuffAnimators[i].SetBool("On", isOnBuff);
                //    //if(isOnBuff)
                //    //{
                //    //    weaponBuffAnimators[i].ResetTrigger("Off");
                //    //    weaponBuffAnimators[i].SetTrigger("On");
                //    //}
                //    //else
                //    //{
                //    //    weaponBuffAnimators[i].ResetTrigger("On");
                //    //    weaponBuffAnimators[i].SetTrigger("Off");
                //    //}
                //}
                //skillOnImage[i].SetActive(isOnBuff);
                //skillOffImage[i].SetActive(!isOnBuff);
            }

            //if(targetEntity.Weapon.WeaponData.upgradeCount <= 0)
            //{
            //    skillCoolTimeImage.fillAmount = 1;
            //    skillCoolTimeText.text = "";
            //    return;
            //}

            //if(!targetEntity.Weapon.WeaponData.IsBuffCoolDown)
            //{
            //    skillCoolTimeImage.fillAmount = 0;
            //    skillCoolTimeText.text = "";
            //    return;
            //}

            //float timer = targetEntity.Weapon.WeaponData.buffCoolTimer;
            //float coolTime = targetEntity.Weapon.WeaponData.buffCoolTime;

            //skillCoolTimeImage.fillAmount = timer / coolTime;
            //skillCoolTimeText.text = Mathf.Ceil(timer).ToString();
        }

        private void updateUltimateGauge()
        {
            if (targetEntity.EntityData.IsOnUltimate)
            {
                if (ultimateGaugeImage.fillAmount == 1)
                {
                    return;
                }
                ultimateGuageAnimator.SetTrigger("Full");
                ultimateGaugeImage.fillAmount = 1;
                return;
            }

            float gauge = targetEntity.EntityData.ultimateGauge;
            float maxGauge = targetEntity.EntityData.ultimateMaxGauge;

            ultimateGaugeImage.fillAmount = 0.01f + gauge / maxGauge * 0.975f;
        }

        public void SetUltimateNormal()
        {
            ultimateGuageAnimator.SetTrigger("Normal");
        }

        public void SetUltimateActive()
        {
            ultimateGuageAnimator.SetTrigger("Active");
        }
    }
}