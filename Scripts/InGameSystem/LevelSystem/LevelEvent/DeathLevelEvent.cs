using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class DeathLevelEvent : LevelEventBase
    {
        public int damage;

        public override void OnLevelEvent(EntityBase entity)
        {
            entity.GetDamage(damage);
        }
    }
}