using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class UpgradeLevelEvent : LevelEventBase
    {
        public string upgradeText;

        public override void OnLevelEvent(EntityBase entity)
        {
            //UIBase baseElement = UIManager.Show(UIList.UpgradeUI);
            //UpgradeUI element = baseElement as UpgradeUI;

            //ToDo:: System Message -> 무기 강화되슝

            UIBase baseElement = UIManager.Instance.GetUI(UIList.SystemUI);
            SystemUI element = baseElement as SystemUI;
            //element.AddSystemMessage(upgradeText);
            element.AddSystemMessage(TextTable.Instance.Get("Text_UpgradeLevelEvent"));

            foreach(var weapon in entity.WeaponList)
            {
                weapon.UpgradeWeapon(weapon.WeaponData.upgradeCount + 1);
            }

            SoundManager.Instance.PlaySound("CheckPoint_On(027)", false);
        }
    }
}