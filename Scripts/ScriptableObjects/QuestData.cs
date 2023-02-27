using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "ScriptableObjects/QuestData", order = 1)]
    public class QuestData : ScriptableObject
    {
        public int questCode;
        public string QuestMainName; 
        public string QuestSubName;

        public string GetMainName()
        {
            string text = TextTable.Instance.Get($"Text_Quest_{questCode}_Main");
            if(text != null && !text.IsNullOrWhitespace())
                return text;
            return QuestMainName;
        }
        public string GetSubName()
        {
            string text = TextTable.Instance.Get($"Text_Quest_{questCode}_Sub");
            if (text != null && !text.IsNullOrWhitespace())
                return text;
            return QuestSubName;
        }
    }
}