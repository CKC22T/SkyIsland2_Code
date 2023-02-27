using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ObjectAnicontrol : MonoBehaviour
    {
        public Vector3 originPos;
        public float sinDir = 0.0f;
        public float moveSpeed = 0.0f;
        public float moveDistance = 0.0f;
        public float rotationSpeed = 0.0f;
        public bool isActive = false;

        [Sirenix.OdinInspector.Button(Name = "Set Random Sin Dir")]
        public void SetSinDir()
        {
            sinDir = Random.Range(0.0f, Mathf.PI);
        }

        private void Awake()
        {
            originPos = transform.localPosition;
            SetSinDir();
        }

        public void SetOriginPos()
        {
            isActive = true;
            sinDir = 0.0f;
            originPos = transform.localPosition;
        }

        void Update()
        {
            if (isActive == false) return;
            sinDir = sinDir + Time.deltaTime * moveSpeed;
            if (sinDir > Mathf.PI) sinDir -= Mathf.PI;
            transform.localPosition = originPos + Vector3.up * Mathf.Sin(sinDir) * moveDistance;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }
}