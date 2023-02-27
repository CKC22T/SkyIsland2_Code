using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PuriWeaponAction : WeaponActionBase
    {
        [SerializeField, TabGroup("Component")] private ParticleSystem puriFBX;
        public override string ActionName { get; protected set; } = "Attack";

        public override void ActionUpdate()
        {
        }

        public override void End()
        {
        }

        public override void Excute()
        {
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            base.AnimationEventAction(eventId);

            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationStart:
                    SoundManager.Instance.PlaySound("Puri_Attack");
                    puriFBX.Play();
                    break;

                case AnimationEventTriggerType.AnimationEnd:
                    entity.SetActionType(ActionType.Idle);
                    break;

                case AnimationEventTriggerType.AnimationAttack:
                    if(PuriController.Instance.AttackTarget == null)
                    {
                        entity.EntityData.targetEntity = null;
                        entity.SetActionType(ActionType.Idle);
                        return; 
                    }
                    PuriController.Instance.AttackTarget.GetDamage(entity.EntityData.attackDamage);

                    //var obj = Instantiate(damageObject, entity.attackPosition.position, entity.attackPosition.rotation);
                    //obj.Owner = entity;
                    //obj.damage = weapon.WeaponData.Damage;
                    break;
            }
        }
    }
}