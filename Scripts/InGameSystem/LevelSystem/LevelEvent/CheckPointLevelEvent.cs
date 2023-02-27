using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class CheckPointLevelEvent : LevelEventBase
    {
        public Transform checkPoint;

        public override void OnLevelEvent(EntityBase entity)
        {
            GameManager.Instance.CheckPoint = checkPoint.position;
            //SoundManager.Instance.PlaySound("CheckPoint_On(027)", false);
        }
    }
}