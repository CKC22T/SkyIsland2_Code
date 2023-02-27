using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SnowballActionDead : ActionBase
    {
        public GameObject snowballHitVFX;
        public GameObject healthItem;
        public override string ActionName { get; protected set; } = "Dead";
        public override void ActionUpdate()
        {
        }

        public override void End()
        {

        }

        public override void Excute()
        {
            SoundManager.Instance.PlayInstance("Homeros_SnowBall_Destroy");
            SnowballActionIdle idleAction = GetComponent<SnowballActionIdle>();
            GameObject snowball = GameObjectPoolManager.Instance.CreateGameObject(snowballHitVFX, transform.position, Quaternion.identity);
            if(idleAction.playerHit == false)
            {
                GameObjectPoolManager.Instance.CreateGameObject(healthItem, transform.position, Quaternion.identity);
            }
            snowball.transform.position = transform.position;
            entity.UseGravity = true;
            entity.EntityData.acceleration = 100.0f;
            entity.EntityData.moveSpeed = 35.0f;
            entity.EntityData.moveDirection = Vector3.zero;
            entity.PhysicsApplication = true;
            idleAction.playerHit = false;

            GameObjectPoolManager.Instance.Release(gameObject);
        }
    }

}