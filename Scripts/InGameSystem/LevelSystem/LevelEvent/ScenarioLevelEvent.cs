using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ScenarioLevelEvent : LevelEventBase
    {
        public bool isVisible = true;
        public int ScenarioCode = 0;

        public override void OnLevelEvent(EntityBase entity)
        {
            if (isVisible)
            {
                GameManager.Instance.StartScenario(ScenarioCode);
            }
            else
            {
                GameManager.Instance.EndScenario(ScenarioCode);
            }
        }
    }
}