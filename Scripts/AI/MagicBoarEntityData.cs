using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Olympus 
{
    public class MagicBoarEntityData : EntityData
{
        // 보어한테 필요한 수치 전용 EntityData

        [field: Title("Boar Param")]
        [SerializeField, ReadOnly] public int boarIndex;
        public MagicBoarAnimationHandler boarAnimHandler;

        [field: Title("Detect Param")]
        public float detectRange { get; private set; } = 6.0f;     // 탐지 사거리.
        public float attackRange { get; private set; } = 3.0f;     // 공격 가능 사거리 
        // 이거 개별로 따로 빼야하나 고민중


        private void Awake() 
        {
            //boarIndex = MagicBoarController.Instance.allBoarSpawnCount;
            //targetEntity = PlayerController.Instance.PlayerEntity;

            //EntityBase boarEntity = GetComponent<EntityBase>();

            //// 객체가 두번 로드 됐다가 한번은 전부 소멸하는데 왜 이렇게 동작하는지 ?
            //if (boarEntity != null &&
            //    Main.Instance.CurrentSceneType == SceneType.TestScene )
            //{
            //    MagicBoarController.Instance.allBoarEntity.Add(boarIndex, boarEntity);
            //    ++MagicBoarController.Instance.allBoarSpawnCount;
            //}
        }

        void Start() 
        {
         
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
