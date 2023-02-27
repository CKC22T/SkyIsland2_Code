using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Olympus
{
    public class UpgradeUI : UIBase
    {
        public Image[] weaponImages;
        public Image[] weaponSkillImages;
        public Image skillLockImage;

        public TextMeshProUGUI currentUpgradeLevelText;
        public TextMeshProUGUI nextUpgradeLevelText;
        public TextMeshProUGUI needStarCountText;

        public Image upgradeImage;

        public Button upgradeButton;
        public WeaponBase currentWeapon;

        public Coroutine upgradeCoroutine = null;

        public GameObject starInfo;
        public Transform upgradeUITransform;
        public Transform offsetPosition;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            SetInfo(PlayerController.Instance.PlayerEntity.Weapon);
        }

        public override void Hide(UnityAction callback = null)
        { 
            base.Hide(callback);
            if(upgradeCoroutine != null)
            {
                StopCoroutine(upgradeCoroutine);
                upgradeImage.fillAmount = 0.0f;
            }
        }

        private void Update()
        {
            upgradeButton.interactable = currentWeapon.WeaponData.upgradeCount < 3 && GameManager.Instance.StarCount >= GameData.UPGRADE_STAR[currentWeapon.WeaponData.upgradeCount];

            if(offsetPosition)
            {
                upgradeUITransform.position = Camera.main.WorldToScreenPoint(offsetPosition.transform.position);
            }
        }

        public void SetInfo(WeaponBase weapon)
        {
            upgradeImage.fillAmount = 0.0f;
            if (upgradeCoroutine != null)
            {
                StopCoroutine(upgradeCoroutine);
            }

            int weaponNumber = -1;
            switch(weapon.WeaponData.WeaponName)
            {
                case "Sword":
                    weaponNumber = 0;
                    break;
                case "Spear":
                    weaponNumber = 1;
                    break;
                case "Hammer":
                    weaponNumber = 2;
                    break;
            }

            currentWeapon = weapon;
            int upgradeLevel = currentWeapon.WeaponData.upgradeCount;
            currentUpgradeLevelText.text = upgradeLevel.ToString();

            if (weaponNumber >= 0)
            {
                for (int i = 0; i < weaponImages.Length; ++i)
                {
                    weaponImages[i].gameObject.SetActive(weaponNumber == i);
                }
                for (int i = 0; i < weaponSkillImages.Length; ++i)
                {
                    weaponSkillImages[i].gameObject.SetActive(weaponNumber == i);
                }

                skillLockImage.gameObject.SetActive(upgradeLevel == 0);
            }

            if (upgradeLevel == 3)
            {
                nextUpgradeLevelText.text = "-";
                needStarCountText.text = "-";
            }
            else
            {
                nextUpgradeLevelText.text = (upgradeLevel + 1).ToString();
                needStarCountText.text = GameData.UPGRADE_STAR[upgradeLevel].ToString() + " °³";
                //upgradeButton.interactable = GameManager.Instance.StarCount >= GameData.UPGRADE_STAR[upgradeLevel];
            }
        }

        public void Upgrade()
        {
            upgradeCoroutine = StartCoroutine(ShowUpgrade());
        }

        public IEnumerator ShowUpgrade()
        {
            float timer = 0.0f;
            while(timer < 1.0f)
            {
                timer += Time.deltaTime;

                upgradeImage.fillAmount = timer;

                yield return null;
            }

            if (skillLockImage.isActiveAndEnabled)
            {
                skillLockImage.gameObject.SetActive(false);
            }

            int upgradeLevel = currentWeapon.WeaponData.upgradeCount;
            GameManager.Instance.UseStar(GameData.UPGRADE_STAR[upgradeLevel]);
            currentWeapon.UpgradeWeapon(upgradeLevel + 1);
        }

        public void ShowStarInfo(bool isVisible)
        {
            starInfo.SetActive(isVisible);
        }
    }
}