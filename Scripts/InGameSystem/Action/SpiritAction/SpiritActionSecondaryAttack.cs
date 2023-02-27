using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SpiritActionSecondaryAttack : ActionBase
    {
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent damageObject;
        public override string ActionName { get; protected set; } = "Attack2";

        public override void ActionUpdate()
        {
        }

        public override void End()
        {
            entity.EntityData.targetEntity = null;
        }

        public override void Excute()
        {
            //SoundManager.Instance.PlayInstance("Spirit_Attack_02");
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    entity.TryChangeActionType(ActionType.Idle);
                    break;
                case AnimationEventTriggerType.AnimationAttack:
                    var obj = GameObjectPoolManager.Instance.CreateGameObject(damageObject, entity.transform.position + Vector3.up, entity.transform.rotation);
                    obj.owner = entity;
                    obj.damage = entity.EntityData.attackDamage;
                    break;
            }
        }
    }
}