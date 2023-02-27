using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class TargetEntityLevelEvent : LevelEventBase
    {
        public bool isSetTargetEntity = true;
        public bool isTriggerEntitySetting = false;
        public EntityBase entity;
        public EntityBase targetEntity;

        public override void OnLevelEvent(EntityBase entity)
        {
            if(isTriggerEntitySetting)
            {
                if (isSetTargetEntity)
                {
                    entity.EntityData.targetEntity = targetEntity;
                }
                else
                {
                    entity.EntityData.targetEntity = null;
                }
                return;
            }

            if(targetEntity == null)
            {
                targetEntity = entity;
            }

            if (isSetTargetEntity)
            {
                this.entity.EntityData.targetEntity = targetEntity;
            }
            else
            {
                this.entity.EntityData.targetEntity = null;
            }
        }
    }
}