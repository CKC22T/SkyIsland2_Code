using Olympus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelEvent : LevelEventBase
{
    public GameObject lockingObject;
    private bool onetimeFlag = false;
    public override void OnLevelEvent(EntityBase entity)
    {
        if (entity != PlayerController.Instance.PlayerEntity)
        {
            return;
        }

        lockingObject.layer = LayerMask.NameToLayer("Enemy");

        BossController.Instance.IsReady = true;

        if (onetimeFlag == true)
        {
            return;
        }

        if (BossController.Instance.bossEntity.ActionType != ActionType.Dead)
        {
            UIManager.Show(UIList.BossHPUI);
        }

        onetimeFlag = true;
    }
}
