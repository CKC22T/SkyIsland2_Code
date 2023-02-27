using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PinoHit : ActionBase
    {
        public override string ActionName { get; protected set; } = "Hit";

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
                ActionName = entity.Weapon.WeaponData.WeaponName + "Hit";
            }
            else
            {
                ActionName = "Hit";
            }

            (UIManager.Instance.GetUI(UIList.CharacterHPUI) as CharacterHPUI).PlayHitAnimation();
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    entity.TryChangeActionType(ActionType.Idle);
                    break;
            }
        }
    }
}