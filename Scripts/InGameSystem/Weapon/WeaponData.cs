using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class WeaponData : MonoBehaviour
    {
        [field: SerializeField, TabGroup("Debug")] public string WeaponName { get; private set; }

        public int attackDamage;
        public float attackSpeed;

        public float buffTimer;
        public bool IsOnBuff => buffTimer > 0.0f;

        public float buffCoolTime;
        public float buffCoolTimer;
        public bool IsBuffCoolDown => buffCoolTimer > 0.0f;

        public float doubleAttackTimer;
        public bool IsOnDoubleAttack => doubleAttackTimer > 0.0f;
        public int upgradeCount;

        public int attackCount = 1;
    }
}