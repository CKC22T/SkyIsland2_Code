using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class RandomAnimationSetting : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(randomSetAnimation());


            IEnumerator randomSetAnimation()
            {
                yield return null;
                yield return null;
                var animator = GetComponentInChildren<Animator>();
                if (animator)
                {
                    animator.Play(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name, 0, Random.Range(0.0f, 1.0f));
                }
            }
        }
    }
}