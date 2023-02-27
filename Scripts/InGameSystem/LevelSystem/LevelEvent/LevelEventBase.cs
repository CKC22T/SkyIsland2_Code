using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public abstract class LevelEventBase : MonoBehaviour
    {
        public bool IsEnd { get; set; } = true;

        public abstract void OnLevelEvent(EntityBase entity);
    }
}