using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ScriptLevelEvent : LevelEventBase
    {
        [SerializeField] private string scriptContents;

        public override void OnLevelEvent(EntityBase entity)
        {
            //TutorialUI ui = UIManager.Show(UIList.TutorialUI) as TutorialUI;
            //ui.ShowScript(scriptContents);
        }
    }
}