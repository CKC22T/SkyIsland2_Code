using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SimpleFollowCamera : MonoBehaviour
    {
        public Vector3 offset;
        public Transform followTarget;
        public float lagSpeed = 3;

        private void Awake()
        {
        }

        private void LateUpdate()
        {
            if (followTarget != null)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    followTarget.position + offset,
                    lagSpeed * Time.deltaTime);
            }
        }
    }
}