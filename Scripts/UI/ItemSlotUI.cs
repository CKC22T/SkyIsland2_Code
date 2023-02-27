using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class ItemSlotUI : UIBase
    {
        public TextMeshProUGUI itemCountText;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            SetItemCount(GameManager.Instance.StarCount);
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        public void  SetItemCount(int count)
        {
            itemCountText.text = count.ToString();
        }
    }
}