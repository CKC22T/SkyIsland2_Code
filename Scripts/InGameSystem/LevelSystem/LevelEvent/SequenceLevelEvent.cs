using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SequenceLevelEvent : LevelEventBase
    {
        [System.Serializable]
        public struct SequenceEventData
        {
            public LevelEventBase levelEvent;
            public float eventTime;
        }

        public List<SequenceEventData> sequenceEventDatas;

        public override void OnLevelEvent(EntityBase entity)
        {
            StartCoroutine(sequence(entity));

            IEnumerator sequence(EntityBase entity)
            {
                foreach (var eventData in sequenceEventDatas)
                {
                    yield return new WaitForSeconds(eventData.eventTime);
                    eventData.levelEvent.OnLevelEvent(entity);
                    yield return new WaitUntil(() => eventData.levelEvent.IsEnd);
                }
            }
        }
    }
}