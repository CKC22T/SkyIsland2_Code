using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BossPlayerExitEvent : LevelEventBase
    {
        public GameObject lockingObject;
        public override void OnLevelEvent(EntityBase entity)
        {
            BossController bossController = BossController.Instance;
            EntityBase bossEntity = bossController.bossEntity;

            bossController.IsReady = false;
            lockingObject.layer = LayerMask.NameToLayer("ObjectForEnemy");

            if (bossController != null)
            {
                bossController.IsReady = false;
                bossEntity.transform.localPosition = new Vector3(183.4009f, 109.8471f, 192.0314f);
            }

            //BossController.Instance.IsReady = false;

            //UIManager.Hide(UIList.BossHPUI);

        }
    }
} 