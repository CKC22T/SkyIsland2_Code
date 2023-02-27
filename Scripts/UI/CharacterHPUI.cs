using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Olympus
{
    public class CharacterHPUI : UIBase
    {
        public EntityBase characterEntity;
        public List<InterfaceAnimateObject> hitAnimations;

        //public TextMeshProUGUI hpText;
        //public Image hpGauge;
        //public Image hpGaugeServe;
        //public float hpGaugeSpeed;

        public GameObject[] hpObject;
        public Image[] hpImage;

        public Coroutine lesshpAnimationRountine = null;

        private new void Awake()
        {
            base.Awake();
            float max = 0;
            foreach (var i in hitAnimations)
            {
                if (i != null)
                {
                    if (max < i.Duration)
                    {
                        max = i.Duration;
                    }
                }
            }

            animationTimer.Reset(max);
        }

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);

            characterEntity = PlayerController.Instance.PlayerEntity;
        }

        public void HpSet()
        {
            if (characterEntity == null)
            {
                return;
            }

            int hp = characterEntity.EntityData.health;

            for (int i = 0; i < hpImage.Length; ++i)
            {
                hpImage[i].enabled = (i < hp);
            }

            if(hp <= 2)
            {
                if(animationRoutine == null)
                {
                    PlayHitAnimation();
                }
            }
        }

        public void MaxHPSet()
        {
            if (characterEntity == null)
            {
                return;
            }

            int size = characterEntity.EntityData.maxHealth / 2;
            if(size > hpObject.Length)
            {
                size = hpObject.Length;
            }

            foreach(var obj in hpObject)
            {
                obj.SetActive(false);
            }

            for(int i = 0; i < size; ++i)
            {
                hpObject[i].SetActive(true);
            }
        }

        public void PlayHitAnimation()
        {
            animationTimer.Reset();
            foreach(var ani in hitAnimations)
            {
                ani?.Play(animationTimer);
            }
            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
                animationRoutine = null;
            }

            animationRoutine = StartCoroutine(playAnimation());
        }

        public void PlayRecoveryAnimation()
        {
            animationTimer.Reset();
            Animate();
        }

        // Update is called once per frame
        void Update()
        {
            HpSet();
            MaxHPSet();
        }
    }
}