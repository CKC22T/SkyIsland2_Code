using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class PinoMove : ActionBase
    {
        public override string ActionName { get; protected set; } = "Move";
        [SerializeField, ReadOnly, TabGroup("Debug")] protected bool possibleActionChange = true;

        public ParticleSystem moveParticle;
        public Vector3 particleOffset;
        public string weaponName = "";
        public bool isLeft = false;

        public bool isWinter = false;

        public override void ActionUpdate()
        {
        }

        public override void ActionFixedUpdate()
        {
            if (entity.EntityData.moveDirection.sqrMagnitude < 0.01f)
            {
                entity.EntityIdle();
                if (entity.physics.LateralVelocity.sqrMagnitude < 0.01f)
                {
                    if(isLeft)
                    {
                        entity.EntityAnimator.SetTrigger("LeftMoveStop");
                    }
                    else
                    {
                        entity.EntityAnimator.SetTrigger("RightMoveStop");
                    }
                }
                //SoundManager.Instance.StopSound("Walk_Dirt");
            }
            else
            {
                //SoundManager.Instance.PlaySound("Walk_Dirt",false);
                entity.EntityMove();
            }
            //entity.EntityMove();
        }

        public override void End()
        {
            SoundManager.Instance.StopSound("Walk_Dirt");
        }

        public override void Excute()
        {
            if (GameDataManager.Instance.stageIslandType == IslandType.Winter)
            {
                isWinter = true;
            }

            if (entity.Weapon && !string.IsNullOrEmpty(entity.Weapon.WeaponData.WeaponName))
            {
                weaponName = entity.Weapon.WeaponData.WeaponName;
                ActionName = weaponName + "Move";
            }
            else
            {
                ActionName = "Move";
            }
            isLeft = false;
            possibleActionChange = false;
            entity.EntityAnimator.ResetTrigger("RightMoveStop");
            entity.EntityAnimator.ResetTrigger("LeftMoveStop");
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch(eventId)
            {
                case AnimationEventTriggerType.AnimationLock:
                    possibleActionChange = false;
                    break;
                case AnimationEventTriggerType.AnimationUnLock:
                    possibleActionChange = true;
                    break;

                case AnimationEventTriggerType.AnimationEffectStart:
                    GameObjectPoolManager.Instance.Release(GameObjectPoolManager.Instance.CreateGameObject(moveParticle.gameObject, entity.transform.position + particleOffset, entity.transform.rotation), 0.5f);
                    if (entity.physics.GroundObject)
                    {
                        if (entity.physics.GroundObject.tag.Equals("Stone"))
                        {
                            SoundManager.Instance.PlaySound("Walk_Stone");
                        }
                        else
                        {
                            if (isWinter)
                            {
                                SoundManager.Instance.PlaySound("Walk_Snow");
                            }
                            else
                            {
                                SoundManager.Instance.PlaySound("Walk_Dirt");
                            }
                        }
                    }
                    isLeft = !isLeft;
                    break;
                case AnimationEventTriggerType.AnimationEnd:
                    entity.TryChangeActionType(ActionType.Idle);
                    break;
            }
        }

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            if (changeActionType == ActionType.Move)
            {
                return possibleActionChange;
            }

            return true;
        }
    }
}