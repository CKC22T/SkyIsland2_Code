using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ExcludeObjectLevelEvent : LevelEventBase
    {
        public override void OnLevelEvent(EntityBase entity)
        {
            if (entity == PlayerController.Instance.PlayerEntity)
            {
                PlayerCamera.Instance.VelocityTracking = true;
            }
            entity.transform.parent = null;
        }
    }
}