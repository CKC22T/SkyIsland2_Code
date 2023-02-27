using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BoarActionMove: ActionBase
    {
        public override string ActionName { get; protected set; } = "Move";

        [field: Title("Angle Time")]
        [Tooltip("새로운 방향을 다시 설정하기까지 걸리는 시간")]
        public float minAngleTime = 0.5f;
        public float maxAngleTime = 2.0f;

        public override void ActionUpdate()
        {
            //if (entity.EntityData.IsAIPatternPossible)
            //{
            //    //플레이어 위치 방향의 시야각을 구해서 이동 거리와 지점을 설정한다.
            //    Vector3 boarToPlayerDirection = MagicBoarController.Instance.GetBoarToTargetDirection(entity);

            //    // 시야각 내에서 랜덤한 각도를 설정한 뒤
            //    float viewAngle = MagicBoarController.Instance.boarAngle * 0.5f;
            //    float randomAngle = Random.Range(-viewAngle, viewAngle);
            //    MagicBoarController.Instance.currentAngle = randomAngle;

            //    Vector3 boarRandomDirection = Quaternion.AngleAxis(randomAngle, Vector3.up) * boarToPlayerDirection;
            //    entity.EntityData.moveDirection = boarRandomDirection.normalized;
            //    entity.EntityData.AIPatternDelay(Random.Range(minAngleTime, maxAngleTime));
            //}
          
            //entity.EntityMove();
        }


        public override void End()
        {
        }

        public override void Excute()
        {

        }
    }
}