using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BoarActionAttack : ActionBase
    {
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent damageObject;
        public override string ActionName { get; protected set; } = "Attack";

        public bool isMove;

        public float moveSpeed;
        public float acceleration;

        public override void ActionUpdate()
        {
        }

        public override void ActionFixedUpdate()
        {
            if(isMove)
            {
                entity.EntityData.moveDirection = entity.transform.forward;
                entity.EntityMove(0.0f, acceleration, moveSpeed);
            }
        }

        public override void End()
        {
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
                    isMove = true;
                    SoundManager.Instance.PlayInstance("MagicBore_Attack_01");
                    break;
                case AnimationEventTriggerType.AnimationEnd:
                    entity.TryChangeActionType(ActionType.Idle);
                    break;
                case AnimationEventTriggerType.AnimationAttack:
                    var obj = GameObjectPoolManager.Instance.CreateGameObject(damageObject, entity.AttackPosition.position, entity.transform.rotation);
                    obj.owner = entity;
                    obj.damage = entity.EntityData.attackDamage;

                    isMove = false;
                    break;
            }

        }
    }
}