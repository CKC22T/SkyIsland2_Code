using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Olympus
{
    public class BoarActionDead : ActionBase
    {
        public override string ActionName { get; protected set; } = "Dead";

        public override void ActionUpdate()
        {
           
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            SoundManager.Instance.PlayInstance("MagicBore_Dead");

            foreach (var dropData in entity.EntityData.dropItemTable)
            {
                float percent = Random.Range(0.0f, 100.0f);
                if (percent <= dropData.dropPercentage)
                {
                    Instantiate(dropData.item, entity.transform.position + Vector3.up * 2.0f, entity.transform.rotation);
                }
            }

            Destroy(entity.gameObject);
        }
    }
}