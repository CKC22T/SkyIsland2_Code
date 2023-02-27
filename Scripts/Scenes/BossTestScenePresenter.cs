using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BossTestScenePresenter : MonoBehaviour
    {
        public static BossTestScenePresenter Instance { get; private set; }
        public EntityBase playerEntity;

        public Transform checkPoint;

        /// <summary>
        /// Prototype and Testing Code Section
        /// </summary>
        public EntityBase bossProtoTypeEntity;

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
            //    UIManager.Show<TitleUI>(UIList.TitleUI);
            GameStart();
        }

        public void GameStart()
        {
            playerEntity.ChangeEntityController(PlayerController.Instance);
            PuriController.Instance.SetFollowTarget(playerEntity);
            //MagicBoarController.Instance.SetFollowTarget(playerEntity);

            bossProtoTypeEntity.ChangeEntityController(BossController.Instance);

            UIManager.Show(UIList.CharacterHPUI);
            UIManager.Show(UIList.QuestInfoUI);

            GameManager.Instance.CheckPoint = checkPoint.position;
        }
    }
}