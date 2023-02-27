using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Olympus
{
    public class BossHPUI : UIBase
    {
        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            data = BossController.Instance.bossEntity.EntityData;

            for (int i = 0; i < 3; i++)
            {
                int phase = BossController.Instance.Phase;
                if (phase == i)
                {
                    hpSliders[i].enabled = true;
                    hpSliders[i].fillAmount = data.health / data.maxHealth;
                }
                else
                {
                    if(phase <= i)
                    {
                        hpSliders[i].enabled = true;
                        hpSliders[i].fillAmount = 1.0f;
                    }
                }
            }

            StartCoroutine(CustomAnimatedTransitionAsync());
        }

        public IEnumerator CustomAnimatedTransitionAsync()
        {
            while (AnimationTimer.IsDone == false)
            {
                float tick = AnimationTimer.Tick();
                float t = AnimationCurve.Evaluate(tick);
                Vector2 interpolated = Vector2.Lerp(KeyPositions.Left, KeyPositions.Right, t);
                AnimationTransform.localPosition = interpolated;

                yield return null;
            }

            AnimationTimer.Reset();
            yield return null;
        }

        public IEnumerator ReverseCustomAnimatedTransitionAsync()
        {
            while (AnimationTimer.IsDone == false)
            {
                float tick = AnimationTimer.Tick();
                float t = 1.0f - AnimationCurve.Evaluate(tick);
                Vector2 interpolated = Vector2.Lerp(KeyPositions.Left, KeyPositions.Right, t);
                AnimationTransform.localPosition = interpolated;

                yield return null;
            }

            AnimationTimer.Reset();
            yield return null;
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        public Image[] hpSliders;
        public DeltaTimer AnimationTimer;
        public AnimationCurve AnimationCurve;
        public SerializablePair<Vector2, Vector2> KeyPositions;
        public RectTransform AnimationTransform;
        public bool removeHpBar = false;
        EntityData data;
        public void Update()
        {
            int phase = BossController.Instance.Phase;
            float factor = (float)data.health / (float)data.maxHealth;

            if (phase != 0)
            {
                hpSliders[phase - 1].enabled = false;
            }

            if (phase == 2 && factor <= 0.0f)
            {
                if (removeHpBar == false)
                {
                    StartCoroutine(ReverseCustomAnimatedTransitionAsync());
                    removeHpBar = true;
                }
            }

            hpSliders[phase].fillAmount = Mathf.Lerp(hpSliders[phase].fillAmount, factor, 0.2f);
        }
    }
}