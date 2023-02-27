using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class NPCMoveLevelEvent : LevelEventBase
    {
        public EntityBase NPCEntity;
        public Transform movePosition;

        public override void OnLevelEvent(EntityBase entity)
        {
            NPCEntity.EntityData.movePosition = movePosition;
        }
    }
}