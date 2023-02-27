using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PuriActionRun : ActionBase
    {
        public override string ActionName { get; protected set; } = "Run";
        [SerializeField, TabGroup("Component")] private float walkDistance = 5.0f;
        [SerializeField, TabGroup("Component")] private float attackRange = 1.0f;

        [SerializeField, TabGroup("Debug")] private float runSpeed = 0.0f;

        private void Start()
        {
            runSpeed = entity.EntityData.moveSpeed * 1.5f;
        }

        public override void ActionUpdate()
        {
            float targetDistance = Vector3.Distance(entity.transform.position, entity.EntityData.targetEntity.transform.position);
            //Player Enemy 차이 조건문 추가해야함
            if (PuriController.Instance.FollowTarget == entity.EntityData.targetEntity)
            {
                if (targetDistance < walkDistance)
                {
                    entity.SetActionType(ActionType.Walk);
                    return;
                }
            }
            else
            {
                if(targetDistance < attackRange)
                {
                    entity.TryChangeActionType(ActionType.Attack);
                    return;
                }
            }

            //인자값으로 속도 조정해도 좋을 것 같다.
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
            entity.EntityData.moveSpeed = runSpeed;
        }
    }
}