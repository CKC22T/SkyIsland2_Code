using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PuriActionEmotion : ActionBase
    {
        public override string ActionName { get; protected set; } = "Emotion";
        [SerializeField, TabGroup("Component")] private float followingDistance = 3.0f;

        public override void ActionUpdate()
        {
            if (entity.EntityData.targetEntity)
            {
                float targetDistance = Vector3.Distance(entity.transform.position, entity.EntityData.targetEntity.transform.position);
                if (targetDistance > followingDistance)
                {
                    entity.SetActionType(ActionType.Walk);
                    return;
                }
            }
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            SoundManager.Instance.PlaySound("Puri_Emotion");
        }
    }
}