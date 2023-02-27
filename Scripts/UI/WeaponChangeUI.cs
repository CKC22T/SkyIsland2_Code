using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Olympus
{
    public class WeaponChangeUI : UIBase
    {
        public EntityBase characterEntity;
        public Animator weaponChangeUIAnimator;
        public Vector3 offset;
        public float autoHideTime = 3.0f;
        public float timer = 0.0f;

        public GameObject[] weaponSelects;
        public GameObject[] weaponDeselects;
        //public Image[] weaponSlots;
        //public Sprite[] weaponSlotImages;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);

            characterEntity = PlayerController.Instance.PlayerEntity;
            timer = 0.0f;
        }

        public void SelectWeapon(int weaponNumber)
        {
            SoundManager.Instance.PlaySound("Player_ChangeWeapon");
            //weaponChangeUIAnimator.SetTrigger("Selected");

            for(int i = 0; i < weaponSelects.Length; ++i)
            {
                weaponSelects[i].SetActive(i == weaponNumber);
                weaponDeselects[i].SetActive(i != weaponNumber);
            }

            //for(int i = 0; i < weaponSlots.Length; ++i)
            //{
            //    weaponSlots[i].sprite = weaponSlotImages[(weaponNumber + 1 + i) % weaponSlotImages.Length];
            //}
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (characterEntity)
            {
                transform.position = Camera.main.WorldToScreenPoint(characterEntity.transform.position) + offset;
            }
            timer += Time.deltaTime;
            if (timer >= autoHideTime)
            {
                UIManager.Hide(UIList.WeaponChangeUI);
            }

            if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                UIManager.Hide(UIList.WeaponChangeUI);
            }
        }
    }
}