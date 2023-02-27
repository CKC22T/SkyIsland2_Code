using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BoxPuriActionIdle : ActionBase
    {
        public override string ActionName { get; protected set; } = "Idle";
        public EntityBase followingEntity;
        public float followingDistance;
        public float idleTime = 1.0f;
        public float idleTimer = 0.0f;

        public override void ActionUpdate()
        {
            idleTimer += Time.deltaTime;
            if(idleTimer < idleTime)
            {
                return;
            }

            if (Vector3.Distance(followingEntity.transform.position, entity.transform.position) > followingDistance)
            {
                //entity.EntityData.moveDirection = (followingEntity.transform.position - entity.transform.position).normalized;
                //entity.EntityRotate();
                //entity.EntityMove();
                entity.EntityData.targetEntity = followingEntity;
            }

            if (entity.EntityData.targetEntity)
            {
                entity.TryChangeActionType(ActionType.Move);
                return;
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            followingEntity = PuriController.Instance.PuriEntity;
            idleTimer = 0.0f;
        }
    }
}