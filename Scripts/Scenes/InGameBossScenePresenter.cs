using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class InGameBossScenePresenter : MonoBehaviour
    {
        public static InGameBossScenePresenter Instance { get; private set; }
        public EntityBase playerEntity;
        public EntityBase bossEntity;
        [SerializeField] private GameObject bossPrefab;
        [SerializeField] private Transform bossSpawnPoint;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

            PlayerController.Instance.InputUnLock();
            playerEntity.ChangeEntityController(PlayerController.Instance);
            playerEntity.EntityData.maxHealth += GameDataManager.Instance.healthLevel * 2;
            playerEntity.EntityData.health = playerEntity.EntityData.maxHealth;
            playerEntity.EntityData.attackSpeed = GameDataManager.Instance.atkSpeedLevel * 0.2f;
            playerEntity.EntityData.criticalPercent = GameDataManager.Instance.criticalLevel * 20.0f;

            playerEntity.WeaponList[0].WeaponData.upgradeCount = GameDataManager.Instance.weaponLevelSword;
            playerEntity.WeaponList[1].WeaponData.upgradeCount = GameDataManager.Instance.weaponLevelSpear;
            playerEntity.WeaponList[2].WeaponData.upgradeCount = GameDataManager.Instance.weaponLevelHammer;

            PuriController.Instance.SetFollowTarget(playerEntity);

            GameManager.Initialize();
            GameDataManager.Instance.Initialize(IslandType.Boss);

        //    if(bossEntity != null)
        //    {
        ///        DestroyImmediate(bossEntity.gameObject);
        //    }

         //   GameObject boss = GameObject.Instantiate(bossPrefab);
        //    boss.transform.position = bossSpawnPoint.position;
        //    boss.transform.rotation = bossSpawnPoint.rotation;

        //    bossEntity = boss.GetComponent<EntityBase>();

            BossController.Instance.ActionLock = false;
            bossEntity.ChangeEntityController(BossController.Instance);

            UIManager.Show(UIList.CharacterHPUI);
            UIManager.Show(UIList.SkillUI);
            UIManager.Show(UIList.CursorUI);

            BGMController.Instance.ChangeBGM("Boss_Cave");
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {

            Instance = null;
        }
    }
}