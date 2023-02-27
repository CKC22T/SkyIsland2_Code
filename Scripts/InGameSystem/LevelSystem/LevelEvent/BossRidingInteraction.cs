using Olympus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossRidingInteraction : LevelEventBase
{
    public override void OnLevelEvent(EntityBase entity)
    {
        BossController bossInstance = BossController.Instance;
        if (bossInstance.IsMountable == true)
        {
            bossInstance.IsGrabbed = true;

            Transform neck = BossController.RidingPoint;

            Vector3 attachPoint = bossInstance.bossEntity.transform.position + new Vector3(0, 1.5f, 0.0f);
            entity.transform.parent = neck;

            entity.transform.localPosition = Vector3.zero;
            entity.transform.localRotation = Quaternion.identity;//bossInstance.bossEntity.transform.rotation;
            entity.UseGravity = false;
            entity.SetActionType(ActionType.MountedFly);
            bossInstance.IsMountable = false;
            PlayerController.Instance.InputLock(LockType.None);
        }
    }
}
