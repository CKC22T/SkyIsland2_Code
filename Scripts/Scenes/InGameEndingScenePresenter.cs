using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class InGameEndingScenePresenter : MonoBehaviour
    {
        public static InGameEndingScenePresenter Instance { get; private set; }

        public EntityBase playerEntity;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            PlayerController.Instance.InputUnLock();
            playerEntity.ChangeEntityController(PlayerController.Instance);
            PuriController.Instance.SetFollowTarget(playerEntity);

            GameDataManager.Instance.Initialize(IslandType.Ending);
            BGMController.Instance.ChangeBGM("End_BackGround");

            GameDataManager.Instance.isClear = true;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}