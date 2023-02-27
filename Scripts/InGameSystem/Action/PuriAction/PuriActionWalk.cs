using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PuriActionWalk : ActionBase
    {
        public override string ActionName { get; protected set; } = "Walk";
        [SerializeField, TabGroup("Component")] private float idleDistance = 1.0f;
        [SerializeField, TabGroup("Component")] private float runDistance = 5.0f;

        [SerializeField, TabGroup("Debug")] private float walkSpeed = 0.0f;

        private void Start()
        {
            walkSpeed = entity.EntityData.moveSpeed;
        }

        public override void ActionUpdate()
        {
            float targetDistance = Vector3.Distance(entity.transform.position, entity.EntityData.targetEntity.transform.position);
            if (targetDistance < idleDistance)
            {
                entity.SetActionType(ActionType.Idle);
                SoundManager.Instance.PlaySound("Puri_Walk", false);

                return;
            }
            if (targetDistance > runDistance)
            {
                entity.SetActionType(ActionType.Run);
                return;
            }

            entity.EntityMove();


            if (entity.EntitySphereCast(entity.EntityData.moveDirection, 1.0f, out var hit))
            {
                entity.EntityJump();
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            entity.EntityData.moveSpeed = walkSpeed;
            SoundManager.Instance.StopSound("Walk_Dirt");

        }
    }
}