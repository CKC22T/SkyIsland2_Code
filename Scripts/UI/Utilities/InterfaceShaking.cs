using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class InterfaceShaking : InterfaceAnimateObject
    {
        [SerializeField] float frequency;
        [SerializeField] float magnitude;
        [SerializeField] AnimationCurve attenuation;

        private bool originFlag = false;
        private Vector2 origin;
        float SampleWave(float elapsedTime, float duration)
        {
            float atten = attenuation.Evaluate(elapsedTime / duration);
            float advancement = Curve.Evaluate(elapsedTime / duration);
            float sampled = Mathf.Sin(elapsedTime * frequency) * (magnitude * (duration - elapsedTime)); //Mathf.Sin(elapsed * frequency) / elapsed * (1.0f / magnitude);

            if (float.IsNaN(sampled) == true)
            {
                sampled = 0.0f;
            }

            return advancement * sampled * (1.0f - atten);
        }

        protected override void Init()
        {
            base.Init();

        }

        protected override void OnAction(float tick, float t, float evaluation)
        {
            if (originFlag == false)
            {
                origin = Target.anchoredPosition;
                originFlag = true;
            }
            float wave = SampleWave(tick, root.animationTimer.Target);

            float x = Random.Range(-1.0f, 1.0f) * wave;
            float y = Random.Range(-1.0f, 1.0f) * wave;

            Target.anchoredPosition = new Vector2(origin.x + x, origin.y + y);
        }

        protected override void OnFinish()
        {
            originFlag = false;
        }
#if UNITY_EDITOR
        protected override void Record()
        {
            return;
        }
#endif
    }
}