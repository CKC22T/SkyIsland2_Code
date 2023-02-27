using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    [CreateAssetMenu(fileName = "ScenarioData", menuName = "ScriptableObjects/ScenarioData", order = 2)]
    public class ScenarioData : ScriptableObject
    {
        public int scenarioCode;
        public EntityType npcCode;

        [TextArea]
        public string scenarioText;
        public int nextScenarioCode;
        public bool possiblePlayerControl;

        public string GetText()
        {
            string text = TextTable.Instance.Get($"Text_Scenario_{scenarioCode}");
            if (text != null && !text.IsNullOrWhitespace())
                return text;
            return scenarioText;
        }
    }
}