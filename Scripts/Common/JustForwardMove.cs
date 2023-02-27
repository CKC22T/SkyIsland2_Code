using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class JustForwardMove : MonoBehaviour
    {
        public float minMoveSpeed;
        public float maxMoveSpeed;
        public float moveSpeed;

        private void OnEnable()
        {
            moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }
}