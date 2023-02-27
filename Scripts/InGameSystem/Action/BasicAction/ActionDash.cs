using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ActionDash : ActionBase
    {
        public override string ActionName { get; protected set; } = "Dash";
        public bool possibleActionChange = false;

        public float moveSpeed;
        public float time = 0.5f;
        public AnimationCurve curve;
        public float dashCoolTime;

        private float timer;
        private Vector3 startDir;

        public override void Excute()
        {
            timer = 0;
            entity.EntityData.dashTimer = dashCoolTime;
            entity.EntityData.godModeTimer = 99999f;
            //startDir = entity.EntityData.moveDirection;

            if (entity.Weapon && !string.IsNullOrEmpty(entity.Weapon.WeaponData.WeaponName))
            {
                ActionName = entity.Weapon.WeaponData.WeaponName + "Dash";
            }
            else
            {
                ActionName = "Dash";
            }
        }

        public override void ActionUpdate()
        {
            timer += Time.deltaTime;
        }

        public override void ActionFixedUpdate()
        {
            entity.physics.LateralVelocity = entity.transform.forward * moveSpeed * curve.Evaluate(timer / time);
            //if(!entity.IsGround)
            //{
            //    entity.physics.LateralVelocity *= 0.95f;
            //}
            //Debug.Log("ASDF");
        }

        public override void End()
        {
            timer = 0;
            entity.EntityData.godModeTimer = 0.0f;
            possibleActionChange = false;
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            base.AnimationEventAction(eventId);

            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationLock:
                    possibleActionChange = false;
                    break;
                case AnimationEventTriggerType.AnimationUnLock:
                    possibleActionChange = true;
                    break;

                case AnimationEventTriggerType.AnimationEnd:
                    entity.TryChangeActionType(ActionType.Idle);
                    break;
            }
        }

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            if(changeActionType == ActionType.Jump)
            {
                return false;
            }

            return possibleActionChange;
        }
    }
}