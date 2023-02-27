using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{

    public class Snowball : MonoBehaviour
    {
        private void Awake()
        {
            
        }
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject != BossController.Instance.bossEntity.gameObject)
            {
                Rigidbody rigid = gameObject.GetComponent<Rigidbody>();
                rigid.useGravity = false;

                Destroy(this);
            }   
    }

    }
}