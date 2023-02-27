using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class CinematicBarUI : UIBase
    {
        public override void Hide(UnityAction callback = null)
        {
            //UIManager.Show(UIList.SkillUI);
            base.Hide(callback);
        }

        public override void Show(UnityAction callback = null)
        {
            //UIManager.Hide(UIList.SkillUI);
         //   UIManager.DisplayGroup("GamePlay", true);
            base.Show(callback);
        }
    }
}