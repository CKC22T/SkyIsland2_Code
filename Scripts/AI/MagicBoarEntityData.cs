using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Olympus 
{
    public class MagicBoarEntityData : EntityData
{
        // �������� �ʿ��� ��ġ ���� EntityData

        [field: Title("Boar Param")]
        [SerializeField, ReadOnly] public int boarIndex;
        public MagicBoarAnimationHandler boarAnimHandler;

        [field: Title("Detect Param")]
        public float detectRange { get; private set; } = 6.0f;     // Ž�� ��Ÿ�.
        public float attackRange { get; private set; } = 3.0f;     // ���� ���� ��Ÿ� 
        // �̰� ������ ���� �����ϳ� �����


        private void Awake() 
        {
            //boarIndex = MagicBoarController.Instance.allBoarSpawnCount;
            //targetEntity = PlayerController.Instance.PlayerEntity;

            //EntityBase boarEntity = GetComponent<EntityBase>();

            //// ��ü�� �ι� �ε� �ƴٰ� �ѹ��� ���� �Ҹ��ϴµ� �� �̷��� �����ϴ��� ?
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
