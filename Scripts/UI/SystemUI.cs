using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class SystemUI : UIBase
    {
        public Queue<string> systemMessageQueue = new();
        public TextMeshProUGUI systemMessageText;
        public CanvasGroup canvasGroup;

        public float alphaTime = 1.0f;
        public float showTime = 2.0f;
        private Coroutine systemMessageShowRoutine = null;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            systemMessageShowRoutine = StartCoroutine(SystemMessageShow());
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
            if(systemMessageShowRoutine != null)
            {
                StopCoroutine(systemMessageShowRoutine);
                systemMessageShowRoutine = null;
            }
        }

        [Button]
        public void AddSystemMessage(string message)
        {
            systemMessageQueue.Enqueue(message);
            if(!isActiveAndEnabled)
            {
                UIManager.Show(Id);
            }
        }

        private IEnumerator SystemMessageShow()
        {
            while (systemMessageQueue.TryDequeue(out string systemMessage))
            {
                systemMessageText.text = systemMessage;

                canvasGroup.alpha = 0.0f;
                float alphaTimeScale = 1 / alphaTime;
                while (canvasGroup.alpha < 1.0f)
                {
                    canvasGroup.alpha += Time.deltaTime * alphaTimeScale;
                    yield return null;
                }
                canvasGroup.alpha = 1.0f;

                yield return new WaitForSeconds(showTime);

                while (canvasGroup.alpha > 0.0f)
                {
                    canvasGroup.alpha -= Time.deltaTime * alphaTimeScale;
                    yield return null;
                }
                canvasGroup.alpha = 0.0f;
            }
            UIManager.Hide(Id);
        }
    }
}