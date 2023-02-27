using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class GiveWeaponLevelEvent : LevelEventBase
    {
        public override void OnLevelEvent(EntityBase entity)
        {
            if (PlayerController.Instance.PlayerEntity.Weapon == null)
            {
                PlayerController.Instance.PlayerEntity.SetWeaponChange(0);
            }
            //UIManager.Show(UIList.SkillUI);
        }
    }
}