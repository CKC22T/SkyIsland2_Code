using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PuriActionIdle : ActionBase
    {
        public override string ActionName { get; protected set; } = "Idle";
        [SerializeField, ReadOnly, TabGroup("Debug")] private float idleTime = 0.0f;
        [SerializeField, TabGroup("Component")] private float emotionChangeTime = 5.0f;
        [SerializeField, TabGroup("Component")] private float followingDistance = 3.0f;
        [SerializeField, TabGroup("Component")] private float attackRange = 1.0f;

        public override void ActionUpdate()
        {
            idleTime += Time.deltaTime;
            if (idleTime > emotionChangeTime)
            {
                entity.TryChangeActionType(ActionType.Emotion);
                return;
            }

            if(entity.EntityData.targetEntity == null)
            {
                return;
            }

            float targetDistance = Vector3.Distance(entity.transform.position, entity.EntityData.targetEntity.transform.position);
            if (entity.EntityData.targetEntity == PuriController.Instance.FollowTarget)
            {
                if (targetDistance > followingDistance)
                {
                    entity.SetActionType(ActionType.Walk);
                    return;
                }
            }
            else
            {
                if (Vector3.Distance(entity.transform.position, PuriController.Instance.FollowTarget.transform.position) > followingDistance * 2.0f)
                {
                    PuriController.Instance.AttackTarget = null;
                    entity.EntityData.targetEntity = null;
                    return;
                }
                if (targetDistance < attackRange)
                {
                    if (idleTime > entity.EntityData.attackSpeed)
                    {
                        entity.TryChangeActionType(ActionType.Attack);
                        return;
                    }
                }
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            idleTime = 0.0f;
        }
    }
}