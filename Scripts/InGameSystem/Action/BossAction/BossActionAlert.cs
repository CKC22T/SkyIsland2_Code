using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Olympus
{
    public class BossActionAlert : ActionBase
    {
        public override string ActionName { get; protected set; } = "IsMoving";
        BossController controller;
        public override void ActionUpdate()
        {
            var firstElement = BossController.MoveableTileIndices.First();
            int index = firstElement.Key;

            Vector3 movePoint = BossController.LandingPoints[index].position;
            Vector3 xzPoint = new Vector3(movePoint.x, entity.transform.position.y, movePoint.z);

            Vector3 dir = (xzPoint - entity.transform.position).normalized;

            entity.EntityData.moveDirection = dir;
            entity.EntityMove();

            float distance = Vector3.Distance(xzPoint, entity.transform.position);
            if(distance <= 0.1f)
            {
                entity.SetActionType(ActionType.Move);
                controller.selectedPatternMethod = BossController.MovingPattern;
            }
        }
        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationStart:
                    entity.EntityAnimator.SetBool("IsMoving", false);
                    break;
                case AnimationEventTriggerType.AnimationEnd:
                    entity.EntityAnimator.SetBool("IsMoving", true);
                    break;
            }
        }
        public override void End()
        {
            BossController.ResetStates();

            controller.ActionLock = false;
            PlayerCamera.Instance.trackingTarget = PlayerController.Instance.PlayerEntity.transform;
        }

        public override void Excute()
        {
            entity.EntityData.moveSpeed = 3.0f;
            controller = BossController.Instance;
        }
    }
}