using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PuriController : SingletonBase<PuriController>, IEntityController
    {
        [field: SerializeField] public EntityBase PuriEntity { get; private set; } = null;
        [field: SerializeField] public EntityBase FollowTarget { get; private set; } = null;
        [field: SerializeField] public EntityBase AttackTarget { get; set; } = null;

        [Button]
        public void SetFollowTarget(EntityBase entity)
        {
            FollowTarget = entity;
        }

        public void ConnectController(EntityBase entity)
        {
            PuriEntity = entity;
        }

        public void ControlEntity(EntityBase entity)
        {
            if(entity.EntityData.targetEntity == null)
            {
                entity.EntityData.targetEntity = FollowTarget;
            }
            if (AttackTarget && AttackTarget != entity.EntityData.targetEntity)
            {
                entity.EntityData.targetEntity = AttackTarget;
                entity.TryChangeActionType(ActionType.Run);
            }

            if(entity.EntityData.targetEntity == null)
            {
                return;
            }

            if(Vector3.Distance(PuriEntity.transform.position, FollowTarget.transform.position) > 20.0f)
            {
                entity.EntityWarp(FollowTarget.transform.position - FollowTarget.transform.forward * 3.0f);
                entity.SetActionType(ActionType.Idle);
            }

            Vector3 moveDir = Vector3.zero;
            moveDir = entity.EntityData.targetEntity.transform.position - entity.transform.position;
            moveDir.y = 0.0f;

            entity.EntityData.moveDirection = moveDir.normalized;
            entity.EntityRotate();
        }

        public void DisconnectController(EntityBase entity)
        {
        }
    }
}