using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SwitchTurrectAction : ActionBase
    {
        public override string ActionName { get; protected set; } = "";

        public bool switchOn = true;
        public bool switchToggle = false;
        public bool switchAutoOff = false;
        public float autoOffTimer = 0.0f;
        public float autoOffTime = 3.0f;
        public LevelEventBase OnlevelEvent;
        public LevelEventBase OfflevelEvent;

        public List<MeshRenderer> turretRenderers = new();
        public Material turretOnMaterial;
        public Material turretOffMaterial;

        public override void ActionUpdate()
        {
            if(switchAutoOff)
            {
                autoOffTimer += Time.deltaTime;
                if(autoOffTimer > autoOffTime)
                {
                    OfflevelEvent?.OnLevelEvent(null);
                    entity.SetActionType(ActionType.Idle);
                }
            }
        }

        public override void End()
        {
            foreach(var renderer in turretRenderers)
            {
                renderer.material = turretOffMaterial;
            }
        }

        public override void Excute()
        {
            foreach (var renderer in turretRenderers)
            {
                renderer.material = turretOnMaterial;
            }

            if (switchOn)
            {
                OnlevelEvent?.OnLevelEvent(null);
            }
            else
            {
                OfflevelEvent?.OnLevelEvent(null);
                entity.SetActionType(ActionType.Idle);
            }

            if (switchToggle)
            {
                switchOn = !switchOn;
            }
            if(switchAutoOff)
            {
                autoOffTimer = 0.0f;
            }
        }
    }
}