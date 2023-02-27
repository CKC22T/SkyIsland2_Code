using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class QuestLevelEvent : LevelEventBase
    {
        public bool isVisible = true;
        public int QuestCode = 0;

        public override void OnLevelEvent(EntityBase entity)
        {
            if(isVisible)
            {
                GameManager.Instance.StartQuest(QuestCode);
            }
            else
            {
                GameManager.Instance.EndQuest(QuestCode);
                SoundManager.Instance.PlaySound("UI_Quest");
            }
        }
    }
}