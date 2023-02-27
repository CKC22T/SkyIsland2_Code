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
            //Player Enemy ���� ���ǹ� �߰��ؾ���
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

            //���ڰ����� �ӵ� �����ص� ���� �� ����.
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