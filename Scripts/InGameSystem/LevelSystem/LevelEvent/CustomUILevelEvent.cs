using Olympus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class CustomUILevelEvent : LevelEventBase
    {
        [SerializeField] UIList uiElement;
        [SerializeField] bool show;
        public override void OnLevelEvent(EntityBase entity)
        {
            if (show == true)
            {
                UIManager.Show(uiElement);
            }
            else
            {
                UIManager.Hide(uiElement);
            }
        }
    }
}