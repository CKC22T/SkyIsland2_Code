using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SpiritActionAttack : ActionBase
    {
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent damageObject;
        public override string ActionName { get; protected set; } = "Attack1";

        public override void ActionUpdate()
        {
        }

        public override void End()
        {
            entity.EntityData.targetEntity = null;
        }

        public override void Excute()
        {
            Vector3 dir = entity.EntityData.targetEntity.transform.position - entity.transform.position;
            dir.y = 0;
            entity.EntityData.moveDirection = dir.normalized;
            entity.EntityRotate(36000);
            //SoundManager.Instance.PlayInstance("Spirit_Attack_01");
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch(eventId)
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