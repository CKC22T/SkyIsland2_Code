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
        [Tooltip("���ο� ������ �ٽ� �����ϱ���� �ɸ��� �ð�")]
        public float minAngleTime = 0.5f;
        public float maxAngleTime = 2.0f;

        public override void ActionUpdate()
        {
            //if (entity.EntityData.IsAIPatternPossible)
            //{
            //    //�÷��̾� ��ġ ������ �þ߰��� ���ؼ� �̵� �Ÿ��� ������ �����Ѵ�.
            //    Vector3 boarToPlayerDirection = MagicBoarController.Instance.GetBoarToTargetDirection(entity);

            //    // �þ߰� ������ ������ ������ ������ ��
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