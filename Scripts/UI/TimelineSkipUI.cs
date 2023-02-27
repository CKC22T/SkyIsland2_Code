using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Olympus
{
    public class TimelineSkipUI : UIBase
    {
        public Image skipImage;
        public GameObject skipObject;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        public void SetSkipFill(float amount)
        {
            skipImage.fillAmount = amount;
            skipObject.SetActive(skipImage.fillAmount > 0.0f);
        }
    }
}