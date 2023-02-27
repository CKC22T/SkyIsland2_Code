using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    //물리 데이터는 따로 빼는게 좋을꺼 같다.
    public class EntityData : MonoBehaviour
    {
        // Movement Param
        public Transform movePosition;
        public Vector3 moveDirection;
        public EntityBase targetEntity;

        public float deceleration = 100.0f;
        public float turningDrag = 35.0f;
        public float acceleration = 35;
        public float moveSpeed = 8;

        public float rotateSpeed = 540;

        public float jumpHeight = 5.0f;
        public float gravityScale = 4.0f;
        //

        public float dashTimer = 0.0f;
        public float ultimateGauge = 0.0f;
        public float ultimateChargeSpeed = 0.5f;
        public float ultimateMaxGauge = 10.0f;
        public bool IsOnUltimate => ultimateGauge >= ultimateMaxGauge;

        //

        public int health;
        public int maxHealth;
        public float godModeTimer = 0.0f;
        
        public int attackDamage;
        public float attackSpeed;
        public float criticalPercent;
        public float criticalDamage;

        public string entityName;

        //public int skillUseCount;
        //public int maxSkillUseCount;
        //public float skillCooltime;

        //public int dashUseCount;
        //public int maxDashUseCount;
        //public float dashCooltime;

        //DropTable
        [System.Serializable]
        public struct DropItemData
        {
            public ItemBase item;
            public float dropPercentage;
        }

        public List<DropItemData> dropItemTable;

        public List<string> baseScript = new();
        public List<string> scenarioBaseScript = new();
    }
}