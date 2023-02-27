using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PuriActionDead : ActionBase
    {
        public override string ActionName { get; protected set; } = "Idle";

        public float deathTime = 1.0f;
        public float timer = 0.0f;

        public override void ActionUpdate()
        {
            timer += Time.deltaTime;
            if(timer >= deathTime)
            {
                EntityBase followTarget = PuriController.Instance.FollowTarget;
                entity.EntityWarp(followTarget.transform.position - followTarget.transform.forward * 3.0f);
                entity.SetActionType(ActionType.Idle);
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            timer = 0.0f;
        }

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            return false;
        }
    }
}