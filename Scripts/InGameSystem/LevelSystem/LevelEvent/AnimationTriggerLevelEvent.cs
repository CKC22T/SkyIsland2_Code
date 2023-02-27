using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class AnimationTriggerLevelEvent : LevelEventBase
    {
        [SerializeField] private Animator animator;
        public string triggerName;

        public bool isSetTrigger;

        public override void OnLevelEvent(EntityBase entity)
        {
            if (isSetTrigger)
            {
                animator.SetTrigger(triggerName);
            }
            else
            {
                animator.ResetTrigger(triggerName);
            }
        }
    }
}