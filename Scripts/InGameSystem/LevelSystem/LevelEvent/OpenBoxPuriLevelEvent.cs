using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class OpenBoxPuriLevelEvent : LevelEventBase
    {
        public int boxId = 0;

        public override void OnLevelEvent(EntityBase entity)
        {
            GameDataManager.Instance.OpenBoxPuri(boxId);
            SoundManager.Instance.PlaySound("Box_Open");
        }
    }
}