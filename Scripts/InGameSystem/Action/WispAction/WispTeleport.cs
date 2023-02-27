using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class WispTeleport : ActionBase
    {
        public override string ActionName { get; protected set; } = "Teleport";

        [SerializeField, TabGroup("Component")] private ParticleSystem teleportStartEffect;
        [SerializeField, TabGroup("Component")] private ParticleSystem teleportEndEffect;

        public override void ActionUpdate()
        {
        }

        public override void End()
        {
            entity.EntityAnimator.ResetTrigger("TeleportEnd");
        }

        public override void Excute()
        {
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch(eventId)
            {
                case AnimationEventTriggerType.AnimationStart:
                    break;
                case AnimationEventTriggerType.AnimationEnd:
                    entity.TryChangeActionType(ActionType.Idle);
                    break;

                case AnimationEventTriggerType.AnimationEffectStart:
                    Vector3 dir = entity.EntityData.targetEntity.transform.position - entity.transform.position;
                    dir.y = 0;
                    dir.Normalize();
                    entity.EntityData.moveDirection = dir;
                    entity.EntityRotate(54000);

                    Vector3 warpPosition = entity.transform.position - entity.transform.forward * 5.0f;
                    if(Physics.Raycast(entity.CenterPosition.position, -dir, out var hit, 5.0f))
                    {
                        warpPosition = entity.transform.position - dir * hit.distance;
                    }
                    if (/*Physics.CheckSphere(warpPosition + Vector3.up * 1.0f, 0.3f) ||*/ !Physics.CheckSphere(warpPosition + Vector3.down * 0.5f, 0.3f))
                    {
                        warpPosition = entity.transform.position;
                    }

                    StartCoroutine(Teleport(warpPosition));
                    break;
            }
        }

        private IEnumerator Teleport(Vector3 position)
        {
            teleportStartEffect.Play();
            SoundManager.Instance.PlayInstance("Wisp_Teleport");
            entity.EntityWarp(position + Vector3.up * 50000);

            yield return new WaitForSeconds(0.2f);
            entity.EntityAnimator.SetTrigger("TeleportEnd");
            entity.EntityWarp(position);
            entity.EntityModel.gameObject.SetActive(true);
            teleportEndEffect.Play();
        }
    }
}