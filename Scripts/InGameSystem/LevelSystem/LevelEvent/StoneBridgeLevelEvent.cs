using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class StoneBridgeLevelEvent : LevelEventBase
    {
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private List<StoneBridge> stoneBridges = new();
        [SerializeField] private float reactDistance;

        [Button(Name = "Set Sorting")]
        public void SetSorting()
        {
            stoneBridges.Sort((bridgeA, bridgeB) =>
            {
                int i = 1;
                if (Vector3.Distance(bridgeA.destination.localPosition, startPoint.position) >
                Vector3.Distance(bridgeB.destination.localPosition, startPoint.position))
                    i = -1;

                return i;
            });
        }

        public override void OnLevelEvent(EntityBase entity)
        {
            foreach(var stone in stoneBridges)
            {
                float distance = Vector3.Distance(entity.transform.position, stone.destination.position);
                if(distance < reactDistance)
                {
                    stone.BridgeOn();

                }
            }
        }
    }
}