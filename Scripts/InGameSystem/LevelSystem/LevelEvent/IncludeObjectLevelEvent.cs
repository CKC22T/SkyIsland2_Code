using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class IncludeObjectLevelEvent : LevelEventBase
    {
        [SerializeField] private GameObject ParentObject;
        public override void OnLevelEvent(EntityBase entity)
        {
            if(entity == PlayerController.Instance.PlayerEntity)
            {
                PlayerCamera.Instance.VelocityTracking = false;
            }
            entity.transform.parent = ParentObject.transform;
        }
    }
}