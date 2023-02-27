using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class WispAttack : ActionBase
    {
        [SerializeField, TabGroup("Component")] private ArrowLevelEvent damageObject;
        public override string ActionName { get; protected set; } = "Attack";

        public ParticleSystem particle;
        public float attackEffectSpeed = 20.0f;

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
                case AnimationEventTriggerType.AnimationStart:
                    SoundManager.Instance.PlayInstance("Wisp_Attack(018)");
                    break;
                case AnimationEventTriggerType.AnimationEnd:
                    entity.TryChangeActionType(ActionType.Idle);
                    break;
                case AnimationEventTriggerType.AnimationAttack:
                    var obj = GameObjectPoolManager.Instance.CreateGameObject(damageObject, entity.AttackPosition.position, entity.AttackPosition.rotation);
                    obj.owner = entity;
                    obj.damage = entity.EntityData.attackDamage;
                    obj.moveSpeed = attackEffectSpeed;

                    GameObjectPoolManager.Instance.Release(GameObjectPoolManager.Instance.CreateGameObject(particle.gameObject, entity.AttackPosition.position, entity.AttackPosition.rotation), 0.5f);
                    break;
            }
        }
    }
}