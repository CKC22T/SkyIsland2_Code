using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class ScriptPopupUI : UIBase
    {
        [SerializeField] private TextMeshProUGUI entityName;
        [SerializeField] private TextMeshProUGUI scriptContents;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            ShowScript("", "");
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        public void ShowScript(string entityName, string scriptContents)
        {
            this.entityName.SetText(entityName);
            this.scriptContents.SetText(scriptContents);
        }

        public void Update()
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                UIManager.Hide(UIList.ScriptPopupUI);
            }
        }
    }
}