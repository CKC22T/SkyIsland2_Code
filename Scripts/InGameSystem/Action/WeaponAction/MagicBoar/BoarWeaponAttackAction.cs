using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BoarWeaponAttackAction : WeaponActionBase
    {
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent damageObject;
        public override string ActionName { get; protected set; } = "Attack";

        public override void ActionUpdate()
        {
            //entity.EntityData.moveDirection = MagicBoarController.Instance.GetBoarToTargetDirection(entity);
            //entity.EntityMove(entity.EntityData.turningDrag, entity.EntityData.acceleration, entity.EntityData.moveSpeed *0.0f);
        }

        public override void End()
        {
        }

        public override void Excute()
        {
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            //base.AnimationEventAction(eventId);

            //switch (eventId)
            //{
            //    case AnimationEventTriggerType.AnimationEnd:
            //        entity.SetActionType(ActionType.Skill);
            //        break;
            //}
        }
    }
}