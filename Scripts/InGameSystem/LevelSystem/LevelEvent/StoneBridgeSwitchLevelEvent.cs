using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class StoneBridgeSwitchLevelEvent : LevelEventBase
    {
        [SerializeField] private StoneBridge startPoint;
        [SerializeField] private List<StoneBridge> stoneBridges = new();
        public bool isbridgeOn = true;
        public float bridgeActiveDelay = 0.1f;

        [Button(Name = "Set Sorting")]
        public void SetSorting()
        {
            stoneBridges.Sort((bridgeA, bridgeB) =>
            {
                int i = 1;
                if (Vector3.Distance(bridgeA.destination.position, startPoint.destination.position) >
                Vector3.Distance(bridgeB.destination.position, startPoint.destination.position))
                    i = -1;

                return i;
            });
        }

        public override void OnLevelEvent(EntityBase entity)
        {
            if (isbridgeOn)
            {
                SoundManager.Instance.PlaySound("Bridge_On", false);
                StartCoroutine(bridgeOn());


                IEnumerator bridgeOn()
                {
                    foreach (var stone in stoneBridges)
                    {
                        stone.BridgeOn();
                        yield return new WaitForSeconds(bridgeActiveDelay);
                    }
                }
            }
            else
            {
                SoundManager.Instance.StopSound("Bridge_On");
                StartCoroutine(bridgeOff());

                IEnumerator bridgeOff()
                {
                    foreach (var stone in stoneBridges)
                    {
                        stone.BridgeOff();
                        yield return new WaitForSeconds(bridgeActiveDelay);
                    }
                }
            }
        }
    }
}