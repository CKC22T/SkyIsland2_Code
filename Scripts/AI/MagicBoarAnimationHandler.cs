using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;
using static UnityEngine.EventSystems.EventTrigger;

// Test code for State transition
namespace Olympus
{
    public class MagicBoarAnimationHandler : MonoBehaviour
    {
        // Ÿ�� ����ġ�� ���ŷο��� �׸�..
        private MagicBoarController targetController;

        #region Condition
        [SerializeField, ReadOnly] public bool allStop = false;

        /*[SerializeField, ReadOnly] */
        public bool canMove = false;
        /*[SerializeField, ReadOnly] */
        public bool isMove = false;

        /*[SerializeField, ReadOnly] */
        public bool canChase = false;
        /*[SerializeField, ReadOnly] */
        public bool isChase = false;

        /*[SerializeField, ReadOnly] */
        public bool canAttack = false;
        /*[SerializeField, ReadOnly] */
        public bool isAttack = false;

        [SerializeField, ReadOnly] public bool isRealRush = false;


        /*[SerializeField, ReadOnly] */

        // // AI patrol ��� �����ϴ� ����
        ///* [SerializeField, ReadOnly] */public bool canDetect = false;
        ///* [SerializeField, ReadOnly] */public bool isDetect = false;
        #endregion

        private void Awake()
        {
            //targetController = MagicBoarController.Instance;
        }


        private void Start()
        {
            EntityBase entity = transform.root.GetComponent<EntityBase>();
            MagicBoarEntityData data = (MagicBoarEntityData)entity.EntityData;

            if (data != null &&
                Main.Instance.CurrentSceneType == SceneType.TestScene)
            {
                //MagicBoarController.Instance.allBoarAnimHandler.Add(data.boarIndex, this);
                data.boarAnimHandler = this;
            }
        }

        public void AllStop()
        {
            allStop = true;
            canMove = false;
            canChase = false;
            canAttack = false;

            isMove = false;
            isChase = false;
            isAttack = false;
            isRealRush = false;
        }

        public void Idle_Start()
        {
            canMove = true;
            canChase = true;
            canAttack = false;

            isMove = false;
            isAttack = false;
            isChase = false;
        }

        public void Patrol_Start()
        {
            canMove = true;
            canChase = true;
            canAttack = false;

            isMove = true;
            isAttack = false;
            isChase = false;
        }

        public void Chase_Start()
        {
            canMove = false;
            canChase = true;
            canAttack = true;

            isMove = false;
            isAttack = false;
            isChase = true;
        }

        public void AttackReady_Start()
        {
            canMove= false;
            canChase = false;
            canAttack = true;

            isMove = false;
            isChase = false;
            isAttack = false;
        }

        public void AttackReady_End()
        {
            // ���� ���� �� �϶��� Attack ���� �������� �Ǵ�.
            isAttack = true;
        }

        public void Attack_End()
        {
            canMove = true;
            canChase = true;
            canAttack = false;

            isMove = false;
            isChase = false;
            isAttack = false;
        }


        public void Rush_Start()
        {
            // ���� ���� �� �϶��� Attack ���� �������� �Ǵ�.
            isRealRush = true;
        }

        public void Rush_End()
        {
            // ���� ���� �� �϶��� Attack ���� �������� �Ǵ�.
            isRealRush = false;
        }

    }

}
