using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class EntitySpawnLevelEvent : LevelEventBase
    {
        [System.Serializable]
        public struct SpawnData
        {
            public EntityBase spawnEntity;
            public Transform spawnTransform;
            public float spawnTime;
        }

        public List<SpawnData> spawnDatas = new();
        public List<EntityBase> spawnEntityList = new();

        public override void OnLevelEvent(EntityBase entity)
        {
            IsEnd = false;

            StartCoroutine(spawn(entity));

            IEnumerator spawn(EntityBase entity)
            {
                foreach (var spawnEntity in spawnEntityList)
                {
                    spawnEntity.entityDeadCallback = EntityDead;
                    spawnEntity.EntityData.gravityScale = 4.0f;
                }

                foreach (var sd in spawnDatas)
                {
                    yield return new WaitForSeconds(sd.spawnTime);
                    if (sd.spawnEntity == null)
                    {
                        LogUtil.LogError("SpawnData : SpawnEntity NULL");
                        continue;
                    }
                    if (sd.spawnTransform == null)
                    {
                        LogUtil.LogError("SpawnData : SpawnTransform NULL");
                        continue;
                    }

                    var obj = GameObjectPoolManager.Instance.CreateGameObject(sd.spawnEntity, sd.spawnTransform.position, sd.spawnTransform.rotation);
                    obj.entityDeadCallback = EntityDead;
                    spawnEntityList.Add(obj);
                }
            }
        }

        public void EntityDead(EntityBase entity)
        {
            spawnEntityList.Remove(entity);
            if(spawnEntityList.Count == 0)
            {
                IsEnd = true;
            }
        }
    }
}