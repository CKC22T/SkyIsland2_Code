using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PhysicsVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigid;
        other.isTrigger = false;
        if(other.TryGetComponent<Rigidbody>(out rigid) == true)
        {
            rigid.isKinematic = false;
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    Rigidbody rigid;
    //    other.isTrigger = false;
    //    if (other.TryGetComponent<Rigidbody>(out rigid) == true)
    //    {
    //        rigid.isKinematic = false;
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rigid;
        other.isTrigger = true;
        if (other.TryGetComponent<Rigidbody>(out rigid) == true)
        {
            rigid.isKinematic = true;
        }
    }
}
