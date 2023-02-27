using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class PhotoModeUI : UIBase
    {
        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);

            //Canvas canvas = GetComponent<Canvas>();
            //canvas.targetDisplay = 1;
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);

        }
    }
}