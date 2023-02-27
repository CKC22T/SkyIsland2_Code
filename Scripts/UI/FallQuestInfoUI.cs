using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    //Quest 구조체 혹은 클래스가 필요할 것 같다.
    public class FallQuestInfoUI : UIBase
    {
        public TextMeshProUGUI questMainNameText;
        public TextMeshProUGUI questSubNameText;

        public int parts = 0;

        public int[] partsToScenarioCode;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);

            parts = 0;
            TextTable.Instance.AddEvent(this, () =>
            {
                UpdateText();
            });
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        public void SetQuestName(string questMainName, string questSubName)
        {
            questMainNameText.text = questMainName;
            questSubNameText.text = questSubName;
        }

        public void GetParts()
        {
            GameManager.Instance.StartScenario(partsToScenarioCode[parts++]);
            UpdateText();
        }
        private void UpdateText()
        {
            questMainNameText.text = TextTable.Instance.Get("Text_FallQuest_Main");
            if (parts < 3)
                questSubNameText.text = string.Format(TextTable.Instance.Get("Text_FallQuest_Sub_1"), parts);
            else
                questSubNameText.text = string.Format(TextTable.Instance.Get("Text_FallQuest_Sub_2"), parts);
        }
    }
}