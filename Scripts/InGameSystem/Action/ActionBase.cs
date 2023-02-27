using Sirenix.OdinInspector;
using UnityEngine;

namespace Olympus
{
    public abstract class ActionBase : MonoBehaviour
    {
        [SerializeField, ReadOnly, TabGroup("Debug")] protected EntityBase entity;
        [SerializeField, ReadOnly, TabGroup("Debug")] public abstract string ActionName { get; protected set; }

        public void Awake()
        {
            entity = GetComponent<EntityBase>();
        }

        public abstract void Excute();
        public abstract void End();
        public abstract void ActionUpdate();
        public virtual void ActionFixedUpdate() { }
        public virtual bool TryChangeActionType(ActionType changeActionType) { return true; }

        public virtual void AnimationEventAction(AnimationEventTriggerType eventId)
        {

        }
    }
}