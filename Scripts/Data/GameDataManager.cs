using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Olympus
{
    public class GameDataManager : SingletonBase<GameDataManager>
    {
        // 무기 별 강화단계
        public int weaponLevelSword = 1;
        public int weaponLevelSpear = 1;
        public int weaponLevelHammer = 1;

        public int spawnPlace = 0;  // 부활 좌표
        public int fallingSpawnPlace = 0;  // 낙사 부활 좌표

        public int itemCount = 0;   // 강화 재화 소지 개수
        public int presentScore = 0;   // 현재 획득 점수
        public int presentTime = 0;    // 현재 진행 시간
        public int deathCount = 0; // 사망 횟수
        public int remainHealth = 0;

        public int healthLevel = 0;  // 플레이어 체력 성장 정보
        public int criticalLevel = 0; // 플레이어 치명타 성장 정보
        public int atkSpeedLevel = 0; // 플레이어 공격속도 성장 정보

        // Stage 정보
        public IslandType stageIslandType = IslandType.None;
        public Dictionary<EntityType, int> enemyKillCounts = new Dictionary<EntityType, int> {
            {EntityType.MagicBoar, 0 },
            {EntityType.Wisp, 0 },
            {EntityType.Spirit, 0 },
            {EntityType.FlowerTrap, 0 },
        };

        // GameEnd 정보
        public float totalGameTime = 0.0f;
        public int totalKillEnemyCount = 0;
        //public int totalOpenBoxPuriCount = 0;
        public HashSet<int> openBoxChecksum = new();
        [ShowInInspector]public int TotalOpenBoxPuriCount => openBoxChecksum.Count;
        public bool isClear = false;

        public void ResetData()
        {
            weaponLevelSword = 1;
            weaponLevelSpear = 1;
            weaponLevelHammer = 1;

            healthLevel = 0;
            criticalLevel = 0;
            atkSpeedLevel = 0;

            totalGameTime = 0.0f;
            totalKillEnemyCount = 0;
            openBoxChecksum.Clear();
            isClear = false;

            stageIslandType = IslandType.Spring;
        }

        public IslandType LoadData()
        {
            switch(stageIslandType)
            {
                case IslandType.Spring:
                    weaponLevelSword = 1;
                    weaponLevelSpear = 1;
                    weaponLevelHammer = 1;
                    break;
                case IslandType.Summer:
                    weaponLevelSword = 1;
                    weaponLevelSpear = 1;
                    weaponLevelHammer = 1;
                    break;
                case IslandType.Fall:
                    weaponLevelSword = 2;
                    weaponLevelSpear = 2;
                    weaponLevelHammer = 2;
                    break;
                case IslandType.Winter:
                    weaponLevelSword = 3;
                    weaponLevelSpear = 3;
                    weaponLevelHammer = 3;
                    break;
            }

            return stageIslandType;
        }

        public void Initialize(IslandType currentIslandType)
        {
            stageIslandType = currentIslandType;

            presentScore = 0;
            presentTime = 0;
            deathCount = 0;
            remainHealth = 0;

            foreach (var enemy in enemyKillCounts.Keys.ToList())
            {
                enemyKillCounts[enemy] = 0;
            }
        }

        [Button]
        public void OpenBoxPuri(int boxId)
        {
            openBoxChecksum.Add(boxId);
        }

        public void SetScore()
        {
            remainHealth = PlayerController.Instance.PlayerEntity.EntityData.health;
            presentTime = Mathf.FloorToInt(GameManager.Instance.gameTimer);

            presentScore = 0;
            presentScore += enemyKillCounts[EntityType.MagicBoar] * 100;
            presentScore += enemyKillCounts[EntityType.Wisp] * 300;
            presentScore += enemyKillCounts[EntityType.FlowerTrap] * 200;
            presentScore += enemyKillCounts[EntityType.Spirit] * 500;
            presentScore -= deathCount * 100;
            presentScore += remainHealth * 100;
            presentScore += 10000 - presentTime * 10;

            totalGameTime += GameManager.Instance.gameTimer;
            foreach(var killCount in enemyKillCounts.Values)
            {
                totalKillEnemyCount += killCount;
            }
        }

        public void SaveWeaponLevel(WeaponBase weapon)
        {
            switch (weapon.WeaponData.WeaponName)
            {
                case "Sword":
                    weaponLevelSword = weapon.WeaponData.upgradeCount;
                    break;
                case "Spear":
                    weaponLevelSpear = weapon.WeaponData.upgradeCount;
                    break;
                case "Hammer":
                    weaponLevelHammer = weapon.WeaponData.upgradeCount;
                    break;
            }
        }

        public void Save()
        {
            PlayerPrefs.SetInt("weaponLevelSword", weaponLevelSword);
            PlayerPrefs.SetInt("weaponLevelSpear", weaponLevelSpear);
            PlayerPrefs.SetInt("weaponLevelHammer", weaponLevelHammer);
            PlayerPrefs.SetInt("spawnPlace", spawnPlace);
            PlayerPrefs.SetInt("fallingSpawnPlace", fallingSpawnPlace);
            PlayerPrefs.SetInt("itemCount", itemCount);
            PlayerPrefs.SetInt("presentScore", presentScore);
            PlayerPrefs.SetInt("presentTime", presentTime);
            PlayerPrefs.SetInt("deathCount", deathCount);
            PlayerPrefs.SetInt("remainHealth", remainHealth);
            PlayerPrefs.SetInt("healthLevel", healthLevel);
            PlayerPrefs.SetInt("criticalLevel", criticalLevel);
            PlayerPrefs.SetInt("atkSpeedLevel", atkSpeedLevel);
            PlayerPrefs.SetInt("stageIslandType", (int)stageIslandType);
            foreach (var kv in enemyKillCounts)
            {
                PlayerPrefs.SetInt(kv.Key.ToString() + "KillCount", kv.Value);
            }
        }
        public void Load()
        {
            weaponLevelSword = PlayerPrefs.GetInt("weaponLevelSword", weaponLevelSword);
            weaponLevelSpear = PlayerPrefs.GetInt("weaponLevelSpear", weaponLevelSpear);
            weaponLevelHammer = PlayerPrefs.GetInt("weaponLevelHammer", weaponLevelHammer);
            spawnPlace = PlayerPrefs.GetInt("spawnPlace", spawnPlace);
            fallingSpawnPlace = PlayerPrefs.GetInt("fallingSpawnPlace", fallingSpawnPlace);
            itemCount = PlayerPrefs.GetInt("itemCount", itemCount);
            presentScore = PlayerPrefs.GetInt("presentScore", presentScore);
            presentTime = PlayerPrefs.GetInt("presentTime", presentTime);
            deathCount = PlayerPrefs.GetInt("deathCount", deathCount);
            remainHealth = PlayerPrefs.GetInt("remainHealth", remainHealth);
            healthLevel = PlayerPrefs.GetInt("healthLevel", healthLevel);
            criticalLevel = PlayerPrefs.GetInt("criticalLevel", criticalLevel);
            atkSpeedLevel = PlayerPrefs.GetInt("atkSpeedLevel", atkSpeedLevel);

            for (int i = (int)EntityType.MagicBoar; i <= (int)EntityType.FlowerTrap; ++i)
            {
                EntityType entityType = (EntityType)i;
                enemyKillCounts[(EntityType)i] = PlayerPrefs.GetInt(entityType.ToString() + "KillCount", 0);
            }
        }
    }
}