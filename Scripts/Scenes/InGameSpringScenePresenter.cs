using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class InGameSpringScenePresenter : MonoBehaviour
    {
        public static InGameSpringScenePresenter Instance { get; private set; }

        public LevelEventBase startTimelineLevelEvent;

        public EntityBase playerEntity;
        public Transform fallSpwanPoint;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            bool isStart = true;
            if(GameDataManager.Instance.stageIslandType == IslandType.Fall)
            {
                playerEntity.EntityWarp(fallSpwanPoint.position);
                isStart = false;
            }

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
            WispController.Instance.SetAttackTarget(playerEntity);
            MagicBoarController.Instance.SetAttackTarget(playerEntity);
            SpiritController.Instance.SetAttackTarget(playerEntity);
            FlowerTrapController.Instance.SetAttackTarget(playerEntity);

            GameManager.Initialize();
            GameManager.Instance.gameTimer = 0.0f;
            GameDataManager.Instance.Initialize(IslandType.Spring);

            UIManager.Show(UIList.CharacterHPUI);
            //UIManager.Show(UIList.SkillUI);
            UIManager.Show(UIList.EnemyHUDManagerUI);
            UIManager.Show(UIList.CursorUI);
            UIBase baseElement = UIManager.Show(UIList.StageStartUI);
            StageStartUI element = baseElement as StageStartUI;

            BGMController.Instance.ChangeBGM("BGM_Spring_Epirus");

            if(isStart)
            {
                startTimelineLevelEvent.OnLevelEvent(null);
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}