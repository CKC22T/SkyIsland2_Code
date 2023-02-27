using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public enum AnimationEventTriggerType
    {
        None,
        AnimationStart,
        AnimationAttack,
        AnimationEnd,
        AnimationLock,
        AnimationUnLock,
        AnimationEffectStart,
    }

    public class AnimationEventListener : MonoBehaviour
    {
        public System.Action<AnimationEventTriggerType> OnAnimationEventAction;

        public void OnEventListener(AnimationEventTriggerType eventId)
        {
            OnAnimationEventAction?.Invoke(eventId);
        }
    }
}