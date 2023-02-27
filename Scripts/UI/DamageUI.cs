using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class DamageUI : UIBase
    {
        public CanvasGroup canvasGroup;
        public float fadeOutSpeed = 3.0f;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            canvasGroup.alpha = 1;
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
            canvasGroup.alpha = 0;
        }

        private void Update()
        {
            canvasGroup.alpha -= Time.deltaTime * fadeOutSpeed;
            if(canvasGroup.alpha <= 0)
            {
                UIManager.Hide(Id);
            }
        }
    }
}