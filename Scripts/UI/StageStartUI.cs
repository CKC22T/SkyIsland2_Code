using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class StageStartUI : UIBase
    {
        public CanvasGroup canvasGroup;
        public float showTime = 1.0f;
        public float alphaTime = 1.0f;

        public TextMeshProUGUI islandName;
        public TextMeshProUGUI olympusName;
        public RectTransform islandPointer;

        public override void Show(UnityAction callback = null)
        {
            InitUI();
            base.Show(callback);
            StartCoroutine(hideAsync(showTime, alphaTime));
            PlayerController.Instance.InputLock(LockType.FromGUI);
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
            PlayerController.Instance.InputUnLock(LockType.FromGUI);
        }

        IEnumerator hideAsync(float showTime, float alphaTime)
        {
            canvasGroup.alpha = 1.0f;
            yield return new WaitForSeconds(showTime);
            float alphaSpeed = 1 / alphaTime;
            while(canvasGroup.alpha > 0.0f)
            {
                canvasGroup.alpha -= Time.deltaTime * alphaSpeed;
                yield return null;
            }
            canvasGroup.alpha = 0.0f;
            UIManager.Hide(UIList.StageStartUI);
        }

        public void InitUI()
        {
            IslandType islandType = GameDataManager.Instance.stageIslandType;
            islandName.text = TextTable.Instance.Get(IslandNameInfo.ISLAND_NAME[islandType]);
            olympusName.text = IslandNameInfo.OLYMPUS_NAME[islandType];

            islandPointer.anchoredPosition = new Vector2(-672.0f + (int)islandType * 272.0f, -240.0f);
        }
    }
}