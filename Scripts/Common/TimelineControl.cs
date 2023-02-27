using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Olympus
{
    public class TimelineControl : MonoBehaviour
    {
        [SerializeField] private PlayableDirector creditTimeline;
        private float dt = 0.0f;
        private float duration = 0.0f;
        [SerializeField] private float timelineTimeScale = 1.0f;
        // Start is called before the first frame update
        void Start()
        {
            creditTimeline = GetComponent<PlayableDirector>();
            duration = (float)creditTimeline.duration;
        }

        private void OnEnable()
        {
            dt = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            timelineTimeScale = 1.0f;
            if(Input.anyKey)
            {
                timelineTimeScale = 3.5f;
            }

            dt += Time.deltaTime * timelineTimeScale;
            creditTimeline.time = Mathf.Min(dt, duration);
            creditTimeline.Evaluate();

            if(creditTimeline.time >= duration)
            {
                gameObject.SetActive(false);
            }
        }
    }
}