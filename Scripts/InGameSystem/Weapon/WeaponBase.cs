using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class WeaponBase : MonoBehaviour
    {
        [field: SerializeField, TabGroup("Component")] public GameObject weaponModel;
        [field: SerializeField, TabGroup("Component")] public Animator weaponAnimator;

        [field: SerializeField] public GameObject[] weaponBuffEffects;

        [field: SerializeField] public WeaponActionBase AttackAction { get; private set; }
        [field: SerializeField] public WeaponActionBase SkillAction { get; private set; }

        [field: SerializeField] public WeaponData WeaponData { get; private set; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (WeaponData.IsOnBuff)
            {
                WeaponData.buffTimer -= Time.deltaTime;
                if(!WeaponData.IsOnBuff)
                {
                    weaponAnimator.SetTrigger("OffBuff");
                    for (int i = 0; i < weaponBuffEffects.Length; ++i)
                    {
                        weaponBuffEffects[i].SetActive(false);
                    }
                }
            }
            if (WeaponData.IsOnDoubleAttack)
            {
                WeaponData.doubleAttackTimer -= Time.deltaTime;
            }
            if (WeaponData.IsBuffCoolDown)
            {
                WeaponData.buffCoolTimer -= Time.deltaTime;
            }
        }

        public void OnBuff()
        {
            WeaponData.buffCoolTimer = WeaponData.buffCoolTime;
            CheckBuffAnimation();
        }

        public void UpgradeWeapon(int upgradeCount)
        {
            WeaponData.upgradeCount = upgradeCount;
            CheckBuffAnimation();
            //UpgradeUI ui = UIManager.Instance.GetUI(UIList.UpgradeUI) as UpgradeUI;
            //ui.SetInfo(this);
            GameDataManager.Instance.SaveWeaponLevel(this);
        }

        public void CheckBuffAnimation()
        {
            if(WeaponData.IsOnBuff)
            {
                weaponAnimator.SetTrigger("OnBuff" + WeaponData.upgradeCount);

                for(int i = 0; i < weaponBuffEffects.Length; ++i)
                {
                    weaponBuffEffects[i].SetActive(i == WeaponData.upgradeCount - 1);
                }
            }
            else
            {
                for (int i = 0; i < weaponBuffEffects.Length; ++i)
                {
                    weaponBuffEffects[i].SetActive(false);
                }
            }
        }
    }
}