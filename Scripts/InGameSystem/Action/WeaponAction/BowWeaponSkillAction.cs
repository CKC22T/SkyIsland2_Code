using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BowWeaponSkillAction : WeaponActionBase
    {
        public override string ActionName { get; protected set; } = "BowSkill";

        public override void ActionUpdate()
        {
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            weapon.WeaponData.buffTimer = 10.0f;
            entity.SetActionType(ActionType.Idle);
        }
    }
}