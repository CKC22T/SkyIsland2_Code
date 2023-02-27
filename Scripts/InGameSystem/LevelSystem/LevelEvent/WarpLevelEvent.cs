using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class WarpLevelEvent : LevelEventBase
    {
        public Transform position;
        public EntityBase target;

        public override void OnLevelEvent(EntityBase entity)
        {
            if(target == null)
            {
                target = entity;
            }
            target.EntityWarp(position.position);
        }
    }
}