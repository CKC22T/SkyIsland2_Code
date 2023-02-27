using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class DebrisDisappear : MonoBehaviour
    {
        private float StayDelay = 5;
        private float DisappearDelay = 20;

        private float delay = 0;
        private Vector3 initialLocalSacle = Vector3.one;
        private Vector3 initialLocalPosition;

        public void Start()
        {
            initialLocalSacle = transform.localScale;
            initialLocalPosition = transform.localPosition;
            StartCoroutine(disappear());
        }

        private IEnumerator disappear()
        {
            delay = DisappearDelay;

            yield return new WaitForSeconds(StayDelay);

            while (delay > 0)
            {
                delay -= Time.deltaTime;

                float scale = delay / DisappearDelay;
                transform.localScale = initialLocalSacle * scale;
                yield return null;
            }

            Destroy(gameObject);
        }

        private void OnDisable()
        {
            transform.localPosition = initialLocalPosition;
        }
    }
}