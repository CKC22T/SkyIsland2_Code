using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class WeaponInfoUI : UIBase
    {
        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                UIManager.Hide(UIList.WeaponInfoUI);
            }
        }
    }
}