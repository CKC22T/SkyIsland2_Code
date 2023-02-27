using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PinoIdle : ActionBase
    {
        public override string ActionName { get; protected set; } = "SwordIdle";

        public override void ActionUpdate()
        {
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            if(entity.Weapon && !string.IsNullOrEmpty(entity.Weapon.WeaponData.WeaponName))
            {
                ActionName = entity.Weapon.WeaponData.WeaponName + "Idle";
            }
            else
            {
                ActionName = "Idle";
            }
        }
    }
}