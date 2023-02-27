using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class TestScenePresenter : MonoBehaviour
    {
        public static TestScenePresenter Instance { get; private set; }
        public EntityBase playerEntity;

        public Transform checkPoint;

        /// <summary>
        /// Prototype and Testing Code Section
        /// </summary>
        public EntityBase bossProtoTypeEntity;


        /// <summary>
        /// 
        /// </summary>

        private void Awake()
        {
            Instance = this;
            GameManager.Initialize();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Start()
        {
            UIManager.Show(UIList.OldTitleUI);
        }

        public void GameStart()
        {
            playerEntity.ChangeEntityController(PlayerController.Instance);

            PuriController.Instance.SetFollowTarget(playerEntity);
            WispController.Instance.SetAttackTarget(playerEntity);
            MagicBoarController.Instance.SetAttackTarget(playerEntity);
            SpiritController.Instance.SetAttackTarget(playerEntity);
            FlowerTrapController.Instance.SetAttackTarget(playerEntity);

            //bossProtoTypeEntity.ChangeEntityController(BossController.Instance);

            GameManager.Initialize();

            UIManager.Show(UIList.CharacterHPUI);
            UIManager.Show(UIList.QuestInfoUI);
            UIManager.Show(UIList.SkillUI);
            UIManager.Show(UIList.ItemSlotUI);
            //UIManager.Show<PhotoGridUI>(UIList.PhotoGridUI);
            //GameDataManager.Instance.SceneEntityContainer.Add(bossProtoTypeEntity);

            GameManager.Instance.CheckPoint = checkPoint.position;
        }
    }
}