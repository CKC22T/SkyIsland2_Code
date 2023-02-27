using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SpiritActionDead : ActionBase
    {
        public override string ActionName { get; protected set; } = "Dead";

        public bool autoDestory = true;

        private Collider entityCollider = null;
        [SerializeField] private GameObject deadEffectObject = null;
        public Vector3 deadEffectOffset;
        public Vector3 deadEffectScale;

        public string deadSoundName = "";
        public string bombSoundName = "";

        [SerializeField] private LevelTrigger respawnLevelTrigger;

        private void Start()
        {
            entityCollider = GetComponent<Collider>();
        }

        public override void ActionUpdate()
        {
        }

        public override void End()
        {
            LogUtil.Log($"EntityNumber[{entity.EntityNumber}]   EntityType[{entity.EntityType}] End     //  'ActionDead'");
        }

        public override void Excute()
        {
            LogUtil.Log($"EntityNumber[{entity.EntityNumber}]   EntityType[{entity.EntityType}] Excute  //  'ActionDead'");
            //Destroy(entity.gameObject);
            //isDeadFly = false;
            autoDestory = entity.EntityData.health <= 0;

            if (autoDestory)
            {
                if (!string.IsNullOrEmpty(deadSoundName))
                {
                    SoundManager.Instance.PlaySound(deadSoundName);
                }
                entity.entityDeadCallback?.Invoke(entity);
            }
        }

        //private void OnDisable()
        //{
        //    if(deadEffectObject)
        //    {
        //        deadEffectObject.SetActive(false);
        //    }
        //}

        public override bool TryChangeActionType(ActionType changeActionType)
        {
            return false;
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    entityCollider.isTrigger = false;
                    if (autoDestory)
                    {
                        if (!string.IsNullOrEmpty(bombSoundName))
                        {
                            SoundManager.Instance.PlaySound(bombSoundName);
                        }
                        var effect = GameObjectPoolManager.Instance.CreateGameObject(deadEffectObject, entity.transform.position + deadEffectOffset, entity.transform.rotation * Quaternion.Euler(-90.0f, 0.0f, 0.0f));
                        effect.transform.localScale = deadEffectScale;
                        GameObjectPoolManager.Instance.Release(effect, 5.0f);
                        GameObjectPoolManager.Instance.Release(entity.gameObject);
                    }
                    else
                    {
                        EnemyHUDManagerUI ui = UIManager.Instance.GetUI(UIList.EnemyHUDManagerUI) as EnemyHUDManagerUI;
                        ui.UnRegisterEnemyHud(entity);
                        respawnLevelTrigger.IsTriggerActive.Enter = true;
                    }
                    break;
                case AnimationEventTriggerType.AnimationStart:
                    foreach (var dropData in entity.EntityData.dropItemTable)
                    {
                        for (float dropPercentage = dropData.dropPercentage; dropPercentage > 0.0f; dropPercentage -= 100.0f)
                        {
                            float percent = Random.Range(0.0f, 100.0f);
                            if (percent <= dropPercentage)
                            {
                                GameObjectPoolManager.Instance.CreateGameObject(dropData.item, entity.transform.position + Vector3.up * 2.0f, entity.transform.rotation);
                            }
                        }
                    }
                    if (autoDestory)
                    {
                        //entityCollider.isTrigger = true;
                        //entity.EntityJump(deadJumpPower);
                        //entity.EntityMove(0.0f, deadMoveSpeed * 100f, deadMoveSpeed, false);
                        //isDeadFly = true;
                    }
                    break;
            }
        }
    }
}