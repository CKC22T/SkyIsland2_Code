using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class WispController : SingletonBase<WispController>, IEntityController
    {
        [field: SerializeField] public EntityBase AttackTarget { get; private set; }

        [field: SerializeField] public float detectDistance = 12.0f;

        [Button]
        public void SetAttackTarget(EntityBase entity)
        {
            AttackTarget = entity;
        }

        public void ConnectController(EntityBase entity)
        {
        }

        public void ControlEntity(EntityBase entity)
        {
            if(AttackTarget == null)
            {
                return;
            }

            if(entity.EntityData.targetEntity == null)
            {
                float distance = Vector3.Distance(entity.transform.position, AttackTarget.transform.position);
                if(distance < detectDistance)
                {
                    entity.EntityData.targetEntity = AttackTarget;
                }
            }
        }

        public void DisconnectController(EntityBase entity)
        {
            entity.EntityData.targetEntity = null;
        }
    }
}