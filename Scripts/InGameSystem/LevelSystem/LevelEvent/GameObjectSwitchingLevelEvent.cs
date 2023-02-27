using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class GameObjectSwitchingLevelEvent : LevelEventBase
    {
        public GameObject switchingObject;
        public bool isActive;

        public override void OnLevelEvent(EntityBase entity)
        {
            switchingObject.SetActive(isActive);
        }
    }
}