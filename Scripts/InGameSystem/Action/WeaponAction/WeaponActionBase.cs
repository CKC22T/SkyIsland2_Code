using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public abstract class WeaponActionBase : ActionBase
    {
        [SerializeField, ReadOnly, TabGroup("Debug")] protected WeaponBase weapon;
        [SerializeField, ReadOnly, TabGroup("Debug")] protected bool possibleActionChange = true;
        [SerializeField, ReadOnly, TabGroup("Debug")] protected bool isMove = false;
        [SerializeField, ReadOnly, TabGroup("Debug")] protected bool isDoubleAttack;
        [SerializeField, ReadOnly, TabGroup("Debug")] protected bool isBuffAttack;

        [SerializeField, TabGroup("Component")] protected float moveSpeed;
        [SerializeField, TabGroup("Component")] protected float acceleration;
        [SerializeField, TabGroup("Component")] protected float nextAttackTime = 0.5f;

        public new void Start()
        {
            entity = GetComponentInParent<EntityBase>();
            weapon = GetComponent<WeaponBase>();
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationLock:
                    possibleActionChange = false;
                    break;
                case AnimationEventTriggerType.AnimationUnLock:
                    possibleActionChange = true;
                    break;
            }
        }

        public override void Excute()
        {
            possibleActionChange = false;
            entity.EntityAnimator.speed = 1 + entity.EntityData.attackSpeed;
        }

        public override void End()
        {
            entity.EntityAnimator.speed = 1;
        }

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            if (changeActionType == ActionType.Jump)
            {
                return false;
            }
            return possibleActionChange;
        }
    }
}