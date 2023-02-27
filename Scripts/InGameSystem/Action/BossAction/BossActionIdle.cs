using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BossActionIdle : ActionBase
    {
        BossController controller;
        public override string ActionName { get; protected set; } = "Idle";

        public override void ActionUpdate()
        {
            
            //  LogUtil.Assert(entity.EntityAnimator != null);
            //if (PlayerController.Instance.PlayerEntity == null)
            //{
            //    return;
            //}

            //controller = entity.EntityController as BossController;
            
            //Transform targetTransform = PlayerController.Instance.PlayerEntity.transform;
            //float targetDistance = Vector3.Distance(targetTransform.position, entity.transform.position);

            //float nextActionFactor = Random.Range(0.0f, 1.0f);
            //LogUtil.Log("Action Factor: " + nextActionFactor);
        }

        public override void End()
        {
        //    throw new System.NotImplementedException();
        }

        public override void Excute()
        {
        }
    }
}