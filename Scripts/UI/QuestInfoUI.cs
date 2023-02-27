using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    //Quest 구조체 혹은 클래스가 필요할 것 같다.
    public class QuestInfoUI : UIBase
    {
        public QuestData questInfoData;
        public TextMeshProUGUI questMainNameText;
        public TextMeshProUGUI questSubNameText;

        private new void Awake()
        {
            base.Awake();
            questMainNameText.text = "";
            questSubNameText.text = "";

            TextTable.Instance.AddEvent(this, () => SetQuestName());
        }

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        private void OnDestroy()
        {
            
        }

        public void SetQuestData(QuestData questData)
        {
            questInfoData = questData;
            (UIManager.Instance.GetUI(UIList.SystemUI) as SystemUI).AddSystemMessage(
                    questInfoData.GetMainName() + "\n<color=#E8D9B6><size=20>: " + questInfoData.GetSubName() + "</size></color>"
                );
            SetQuestName();
        }

        public void SetQuestName()
        {
            questMainNameText.text = questInfoData.GetMainName();
            questSubNameText.text = questInfoData.GetSubName();
        }
    }
}