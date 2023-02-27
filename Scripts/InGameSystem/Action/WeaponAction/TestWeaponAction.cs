using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class TestWeaponAction : WeaponActionBase
    {
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent damageObject;
        public override string ActionName { get; protected set; } = "Attack";


        public override void ActionUpdate()
        {
        }

        public override void End()
        {
        }

        public override void Excute()
        {
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            base.AnimationEventAction(eventId);

            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    entity.SetActionType(ActionType.Idle);
                    break;

                case AnimationEventTriggerType.AnimationAttack:
                    var obj = Instantiate(damageObject, entity.AttackPosition.position, entity.AttackPosition.rotation);
                    obj.owner = entity;
                    obj.damage = weapon.WeaponData.attackDamage;
                    break;
            }
        }
    }
}