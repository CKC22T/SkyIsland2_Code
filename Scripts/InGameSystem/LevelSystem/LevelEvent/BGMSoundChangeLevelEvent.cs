using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BGMSoundChangeLevelEvent : LevelEventBase
    {
        [SerializeField] private string soundName = "";

        public override void OnLevelEvent(EntityBase entity)
        {
            BGMController.Instance.ChangeBGM(soundName);
        }
    }
}