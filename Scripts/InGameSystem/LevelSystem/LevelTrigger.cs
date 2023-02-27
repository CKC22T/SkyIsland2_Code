using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class LevelTrigger : MonoBehaviour
    {
        [System.Serializable]
        public struct LevelTriggerOption
        {
            public bool Enter;
            public bool Stay;
            public bool Exit;
        }

        [field: Sirenix.OdinInspector.Title("Level Trigger Option")]
        [field: SerializeField] public LevelTriggerOption IsTriggerActive;
        [field: SerializeField] public LevelTriggerOption IsTriggerRepeat;

        [field: Sirenix.OdinInspector.Title("Level Event List")]
        [field: SerializeField] private List<LevelEventBase> enterLevelEvents = new();
        [field: SerializeField] private List<LevelEventBase> stayLevelEvents = new();
        [field: SerializeField] private List<LevelEventBase> exitLevelEvents = new();

        private void OnTriggerEnter(Collider other)
        {
            if(!IsTriggerActive.Enter)
            {
                return;
            }

            if(other.TryGetComponent(out EntityBase entity))
            {
                IsTriggerActive.Enter = IsTriggerRepeat.Enter;
                foreach (var e in enterLevelEvents)
                {
                    e.OnLevelEvent(entity);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!IsTriggerActive.Stay)
            {
                return;
            }

            if (other.TryGetComponent(out EntityBase entity))
            {
                IsTriggerActive.Stay = IsTriggerRepeat.Stay;
                foreach (var e in stayLevelEvents)
                {
                    e.OnLevelEvent(entity);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsTriggerActive.Exit)
            {
                return;
            }

            if (other.TryGetComponent(out EntityBase entity))
            {
                IsTriggerActive.Exit = IsTriggerRepeat.Exit;
                foreach (var e in exitLevelEvents)
                {
                    e.OnLevelEvent(entity);
                }
            }
        }
    }
}