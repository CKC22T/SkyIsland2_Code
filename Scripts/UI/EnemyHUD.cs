using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Olympus
{
    public class EnemyHUD : MonoBehaviour
    {
        public EntityBase enemy;

        public TextMeshProUGUI enemyNameText;
        public Image enemyHPImage;
        public float widthSize;

        private RectTransform rectTransfrom;

        public void Start()
        {
            rectTransfrom = enemyHPImage.GetComponent<RectTransform>();
            widthSize = rectTransfrom.sizeDelta.x;

            TextTable.Instance.AddEvent(this, () =>
            {
                if (enemy)
                    SetEnemy(enemy);
            });
        }

        public void SetEnemy(EntityBase enemy)
        {
            this.enemy = enemy;
            enemyNameText.text = TextTable.Instance.Get(enemy.EntityData.entityName);
        }

        public void SetHealth()
        {
            Vector2 size = rectTransfrom.sizeDelta;
            size.x = widthSize * enemy.EntityData.health / enemy.EntityData.maxHealth;
            rectTransfrom.sizeDelta = size;
        }

        public void SetPosition()
        {
            transform.position = Camera.main.WorldToScreenPoint(enemy.transform.position + Vector3.up * enemy.Height * 1.5f);
        }
    }
}