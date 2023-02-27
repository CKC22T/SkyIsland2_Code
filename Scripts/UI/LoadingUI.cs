using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class LoadingUI : UIBase
    {
        public TMPro.TextMeshProUGUI loadingText;

        public CanvasGroup canvas;
        public float fadeInSpeed = 1.0f;
        public float fadeOutSpeed = 1.0f;

        public Coroutine fadeRoutine = null;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            //if (fadeRoutine != null)
            //{
            //    StopCoroutine(fadeRoutine);
            //}
            //fadeRoutine = StartCoroutine(FadeIn(callback));
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
            //if (fadeRoutine != null)
            //{
            //    StopCoroutine(fadeRoutine);
            //}
            //fadeRoutine = StartCoroutine(FadeOut(callback));
        }

        //private void StartTween()
        //{
        //    Sequence seq = DOTween.Sequence()
        //        .Append(DOTween.To(x => loadingText.maxVisibleCharacters = (int)x, 0f, loadingText.text.Length, 2f))
        //        .SetDelay(1f)
        //        .SetLoops(-1)
        //        .Play();

        //}

        private IEnumerator FadeIn(UnityAction callback = null)
        {
            while (canvas.alpha < 1.0f)
            {
                canvas.alpha += fadeInSpeed * Time.deltaTime;
                yield return null;
            }
            canvas.alpha = 1.0f;
        }

        private IEnumerator FadeOut(UnityAction callback = null)
        {
            while (canvas.alpha > 0.0f)
            {
                canvas.alpha -= fadeOutSpeed * Time.deltaTime;
                yield return null;
            }
            canvas.alpha = 0.0f;
        }
    }
}