using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class JumpFlowerActionOpenIdle : ActionBase
    {
        public override string ActionName { get; protected set; } = "OpenIdle";
        [SerializeField, TabGroup("Component")] private SphereCollider jumpCollider;

        public override void ActionUpdate()
        {
            if (entity.EntityData.targetEntity == null)
            {
                entity.TryChangeActionType(ActionType.Walk);
            }
        }

        public override void End()
        {
            jumpCollider.enabled = false;
        }

        public override void Excute()
        {
            jumpCollider.enabled = true;
        }
    }
}