using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class FallQuestLevelEvent : LevelEventBase
    {
        public bool isShowUI = false;

        public int parts = 0;
        public int[] partsToScenarioCode;

        public override void OnLevelEvent(EntityBase entity)
        {
            //if(isShowUI)
            //{
            //    UIManager.Show(UIList.FallQuestInfoUI);
            //}
            //else
            //{
            //    gameObject.SetActive(false);
            //    FallQuestInfoUI ui = UIManager.Instance.GetUI(UIList.FallQuestInfoUI) as FallQuestInfoUI;
            //    ui.GetParts();
            //}
            GetParts();
        }

        public void GetParts()
        {
            QuestInfoUI ui = UIManager.Instance.GetUI(UIList.QuestInfoUI) as QuestInfoUI;

            GameManager.Instance.StartScenario(partsToScenarioCode[parts++]);
            ui.questSubNameText.text = string.Format(TextTable.Instance.Get("Text_FallQuest_Sub_1"), parts);

            //if (parts == 3)
            //{
            //    questSubNameText.text = "라리사로 가자";
            //}
        }
    }
}