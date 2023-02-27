using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BoarActionJump : ActionBase
    {
        public override string ActionName { get; protected set; } = "Jump";

        [field: Title("Chase Accel")]
        [Tooltip("플레이어 발견시 가속도 (minValue = 1.0)")]
        public float chaseAccel = 1.2f;

        public override void ActionUpdate()
        {
            //entity.EntityData.moveDirection = MagicBoarController.Instance.GetBoarToTargetDirection(entity);
            entity.EntityMove(entity.EntityData.turningDrag, entity.EntityData.acceleration, entity.EntityData.moveSpeed * chaseAccel);
        }

        public override void End()
        {
        }

        public override void Excute()
        {
       
        }
    }
}