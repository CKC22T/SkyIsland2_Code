using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SnowStorm : MonoBehaviour
    {
        DeltaTimer timer = new(5.25f);
        public Transform Center;
        EntityBase player;
        bool IsInside = false;

        private void Awake()
        {
            player = PlayerController.Instance.PlayerEntity;
        }
        private void Update()
        {
            transform.RotateAround(Center.position, Vector3.up, 0.25f);

            timer.Tick();
            SoundManager.Instance.PlaySound("Homeros_Blizzard", false);

            if (timer.IsDone == true)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == PlayerController.Instance.PlayerEntity.gameObject)
            {
                player.EntityData.moveSpeed -= player.EntityData.moveSpeed * 0.2f;
                IsInside = true;
            }
        }

        private void OnDestroy()
        {
            player.EntityData.moveSpeed = (IsInside == true) ? PlayerController.Instance.originalSpeed : player.EntityData.moveSpeed;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject == PlayerController.Instance.PlayerEntity.gameObject)
            {
                player.GetDamage(1);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == PlayerController.Instance.PlayerEntity.gameObject)
            {
                player.EntityData.moveSpeed = PlayerController.Instance.originalSpeed;
            }
        }
    }
}