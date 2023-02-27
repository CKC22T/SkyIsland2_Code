using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class MiniPuriActionIdle : ActionBase
    {
        public override string ActionName { get; protected set; }

        public override void ActionUpdate()
        {
            if (entity.EntityData.targetEntity)
            {
                entity.EntityData.moveDirection = (entity.transform.position - entity.EntityData.targetEntity.transform.position).normalized;
                entity.EntityRotate();
                entity.EntityMove();
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            entity.EntityAnimator.Play("Idle", 0, Random.Range(0.0f, 1.0f));
        }
    }
}