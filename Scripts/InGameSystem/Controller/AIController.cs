using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class AIController : SingletonBase<AIController>, IEntityController
    {
        public void ConnectController(EntityBase entity)
        {

        }

        public void ControlEntity(EntityBase entity)
        {
            if (entity.EntitySphereCast(entity.transform.rotation.eulerAngles, 2.0f, out var hit))
            {
                
            }

            //if (entity.EntityData.IsAIPatternPossible)
            //{
            //    if (Random.Range(0, 3) > 0)
            //    {
            //        entity.EntityData.movePosition = entity.transform.position + Vector3.right * Random.Range(-1.0f, 1.0f) + Vector3.forward * Random.Range(-1.0f, 1.0f);
            //        Vector3 moveDirection = entity.transform.position - entity.EntityData.movePosition;
            //        moveDirection.y = 0.0f;
            //        entity.EntityData.moveDirection = moveDirection.normalized;
            //        entity.TryChangeActionType(ActionType.Move);
            //        entity.EntityData.AIPatternDelay(Random.Range(0.15f, 0.35f));
            //    }
            //    else
            //    {
            //        entity.EntityData.movePosition = entity.transform.position;
            //        entity.TryChangeActionType(ActionType.Idle);
            //        entity.EntityData.AIPatternDelay(Random.Range(0.6f, 0.8f));
            //    }
            //}
        }

        public void DisconnectController(EntityBase entity)
        {

        }

        private IEnumerator AIPatternDelay(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
        }
    }
}