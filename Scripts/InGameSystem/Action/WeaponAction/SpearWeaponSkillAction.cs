using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SpearWeaponSkillAction : WeaponActionBase
    {
        public override string ActionName { get; protected set; } = "SpearIdle";
        public float buffTime = 8.0f;

        public override void ActionUpdate()
        {
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            weapon.WeaponData.buffTimer = buffTime;
            entity.SetActionType(ActionType.Idle);
        }
    }
}