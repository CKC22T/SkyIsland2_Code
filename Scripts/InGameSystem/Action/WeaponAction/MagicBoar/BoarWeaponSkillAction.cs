using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BoarWeaponSkillAction : WeaponActionBase
    {
        [SerializeField, TabGroup("Component")] private DamageEffectLevelEvent damageObject;
        public override string ActionName { get; protected set; } = "Skill";

        [field:Title("Attack Rush Param")]

        // 이건 AnimationHandler로 빼장 ㅎ
        private float rushDistance = 2.0f;
        private float rushTime = 0.4f;

        [Tooltip ("Test feature : 패턴 종료 후 딜레이 타임 ")]
        public float delayTime = 2.0f;
        private Vector3 rushPosition = Vector3.zero;


        public override void ActionUpdate()
        {
            ////// 실제 공격 하는 모션 시작에 이동값을 준다.
            MagicBoarEntityData boarEntityData = (MagicBoarEntityData)entity.EntityData;
            MagicBoarAnimationHandler animHandler = boarEntityData.boarAnimHandler;

            if (animHandler.isRealRush)
            {
                if (rushTime == 0.4f)
                {
                    // 돌진할 방향을 저장한다.
                    rushPosition = entity.transform.position + (entity.EntityData.moveDirection * rushDistance);
                }

                if (rushTime >= 0.0f)
                {
                    entity.transform.position = Vector3.Lerp(entity.transform.position, rushPosition, Time.deltaTime / rushTime);
                }

                rushTime -= Time.deltaTime;
            }

            // Attack_01 ani 시전 중이지만 움직이면 안되는 경우
            else
            {
                //entity.EntityData.moveDirection = MagicBoarController.Instance.GetBoarToTargetDirection(entity);
                entity.EntityMove(entity.EntityData.turningDrag, entity.EntityData.acceleration, entity.EntityData.moveSpeed * 0.0f);

            }

            #region Legacy
            ////// 실제 공격 하는 모션 시작
            //if (MagicBoarController.Instance.isRush)
            //{
            //    if (MagicBoarController.Instance.rushTime == 0.4f)
            //    {
            //        // 돌진할 방향을 저장한다.
            //        entity.EntityData.moveDirection = MagicBoarController.Instance.GetBoarToTargetDirection(entity);
            //        entity.EntityRotate();
            //        MagicBoarController.Instance.rushPosition = entity.transform.position + (entity.EntityData.moveDirection * MagicBoarController.Instance.rushDistance);
            //    }

            //    if (MagicBoarController.Instance.rushTime >= 0.0f)
            //    {
            //        entity.transform.position = Vector3.Lerp(entity.transform.position, MagicBoarController.Instance.rushPosition, Time.deltaTime / rushTime);
            //    }

            //    MagicBoarController.Instance.rushTime -= Time.deltaTime;
            //}
            #endregion

        }

        public override void End()
        {
            MagicBoarEntityData boarEntityData = (MagicBoarEntityData)entity.EntityData;
            MagicBoarAnimationHandler animHandler = boarEntityData.boarAnimHandler;

            if (!animHandler.isAttack)
            {
                entity.EntityData.moveDirection = Vector3.zero;
                animHandler.allStop = true;

                rushTime = 0.4f;
                rushPosition = Vector3.zero;

                //StartCoroutine(MagicBoarController.Instance.ChaseStop(entity, 2.0f));
            }

            #region Legacy
            //entity.EntityData.moveDirection = Vector3.zero;
            //MagicBoarController.Instance.AllStop = true;

            //MagicBoarController.Instance.AttackEnd();

            ////MagicBoarController.Instance.attackEnd = false;
            //MagicBoarController.Instance.isRush = false;
            //MagicBoarController.Instance.rushTime = 0.4f;

            //StartCoroutine(MagicBoarController.Instance.ChaseStop(2.0f));
            #endregion

        }



        public override void Excute()
        {
          
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            base.AnimationEventAction(eventId);

            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    break;
            }
        }
    }
}