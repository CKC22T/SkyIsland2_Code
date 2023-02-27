using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class NPCActionIdle : ActionBase
    {
        public override string ActionName { get; protected set; } = "Idle";
        [SerializeField, TabGroup("Component")] private float followingDistance = 5.0f;

        public override void ActionUpdate()
        {
            if(entity.EntityData.movePosition)
            {
                entity.SetActionType(ActionType.Move);
            }

            if(entity.EntityData.targetEntity)
            {
                float targetDistance = Vector3.Distance(entity.transform.position, entity.EntityData.targetEntity.transform.position);
                if (targetDistance > followingDistance)
                {
                    entity.SetActionType(ActionType.Move);
                    return;
                }
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
        }
    }
}