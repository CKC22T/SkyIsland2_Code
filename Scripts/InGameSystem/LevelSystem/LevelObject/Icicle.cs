using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class Icicle : MonoBehaviour
    {
        Rigidbody rigid;
        EntityBase bossEntity;
        EntityBase playerEntity;
        public GameObject icicleMesh;
        private BoxCollider collider;
        public ParticleSystem breakVFX;
        public DeltaTimer breakTimer = new DeltaTimer(0.34f);

        private bool onetimeVFXFlag = false;
        private bool onetimeHitFlag = false;
        bool timerFlag = false;
        void Start()
        {
            collider = GetComponent<BoxCollider>();
            rigid = GetComponent<Rigidbody>();
            bossEntity = BossController.Instance.bossEntity;
            playerEntity = PlayerController.Instance.PlayerEntity;

            Vector3 playerPosition = playerEntity.transform.position;
            float distance = Vector3.Distance(bossEntity.transform.position, playerPosition);

            rigid.AddForce(BossController.Instance.TargetDirection * (distance * 2), ForceMode.VelocityChange);
            breakVFX.Stop();
            breakVFX.time = 0;

            transform.LookAt(playerPosition, transform.up);
            transform.Rotate(new Vector3(-90, 0.0f, 0.0f));
            
        }

        private void Update()
        {

        }

        private void OnTriggerStay(Collider other)
        {
            //if (other.gameObject == playerEntity.gameObject)
            //{
            //    playerEntity.GetDamage(1);
            //    return;
            //}

            int layer = LayerMask.NameToLayer("Environment");
            if (other.gameObject.layer.Equals(layer) && onetimeVFXFlag == false)
            {
                rigid.velocity = Vector3.zero;
                icicleMesh.SetActive(false);
                breakVFX.Play();

                bool overlapTest = Physics.CheckBox(transform.position + collider.center, collider.size / 2, transform.rotation, LayerData.LAYER_MASK_PLAYER, QueryTriggerInteraction.UseGlobal);
                if (overlapTest == true)
                {
                    playerEntity.GetDamage(1);
                }
                collider.enabled = false;
            }

            if (other.gameObject == bossEntity.gameObject)
            {
                return;
            }
            if (other.gameObject.GetComponent<Icicle>() != null)
            {
                return;
            }
            SoundManager.Instance.PlaySound("Homeros_Icicle", true);
            //timerFlag = true;
            Destroy(gameObject, 10.0f);
        }
    }
}