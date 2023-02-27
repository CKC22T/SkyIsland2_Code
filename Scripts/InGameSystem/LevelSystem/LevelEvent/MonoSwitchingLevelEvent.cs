using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class MonoSwitchingLevelEvent : LevelEventBase
    {
        public MonoBehaviour mono;
        public bool isOn;

        public override void OnLevelEvent(EntityBase entity)
        {
            mono.enabled = isOn;
        }
    }
}