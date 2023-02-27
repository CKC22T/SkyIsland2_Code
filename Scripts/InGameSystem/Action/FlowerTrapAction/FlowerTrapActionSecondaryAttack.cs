using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class FlowerTrapActionSecondaryAttack : ActionBase
    {
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent damageObject;
        public override string ActionName { get; protected set; } = "Attack2";

        private void Start()
        {
        }

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
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    entity.TryChangeActionType(ActionType.Idle);
                    break;
                case AnimationEventTriggerType.AnimationEffectStart:
                    //SoundManager.Instance.PlayInstance("FlowerTrap_Attack_02");

                    var obj = GameObjectPoolManager.Instance.CreateGameObject(damageObject, entity.transform.position + Vector3.up, entity.transform.rotation);
                    obj.owner = entity;
                    obj.damage = entity.EntityData.attackDamage;
                    break;
            }
        }
    }
}