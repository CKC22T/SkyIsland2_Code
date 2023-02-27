using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BirdSpawner : MonoBehaviour
    {
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private GameObject birdPrefab;

        [SerializeField] private SerializableDictionary<IslandType, Material> islandBirdMaterials = new();
        [SerializeField] private Material birdMaterial;

        private List<SkinnedMeshRenderer> birdObjects = new();
        public bool poolingSetting = false;

        public float spawnTimer = 1.0f;
        public int initSpawnCount = 100;
        
        private Coroutine spawnRountine;

        // Start is called before the first frame update
        void Start()
        {
            //IslandType islandType = GameDataManager.Instance.stageIslandType;
            if (birdMaterial)
            {
                birdPrefab.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = birdMaterial;

                if (poolingSetting)
                {
                    var pool = GameObjectPoolManager.Instance.GetGameObjectPool(birdPrefab);
                    if (pool != null)
                    {
                        foreach (var obj in pool.GetPoolingObjects())
                        {
                            obj.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = birdMaterial;
                        }
                    }
                }
            }

            spawnRountine = StartCoroutine(spawnBirdAsync());
            InitSpawnBird();
        }

        private void OnDestroy()
        {
            StopCoroutine(spawnRountine);
            spawnRountine = null;
        }

        private IEnumerator spawnBirdAsync()
        {
            while (true)
            {
                SpawnBird();
                yield return new WaitForSeconds(spawnTimer);
            }
        }

        private void SpawnBird()
        {
            Vector3 spawnPosition = transform.position + transform.rotation * (new Vector3(Random.Range(0, boxCollider.size.x), Random.Range(0, boxCollider.size.y), 0) - boxCollider.size * 0.5f);
            var obj = GameObjectPoolManager.Instance.CreateGameObject(birdPrefab, spawnPosition, transform.rotation);
        }

        private void InitSpawnBird()
        {
            for(int  i = 0; i < initSpawnCount; ++i)
            {
                Vector3 spawnPosition = transform.position + transform.rotation * (new Vector3(Random.Range(0, boxCollider.size.x), Random.Range(0, boxCollider.size.y), Random.Range(0, boxCollider.size.z)) - boxCollider.size * 0.5f);
                var obj = GameObjectPoolManager.Instance.CreateGameObject(birdPrefab, spawnPosition, transform.rotation);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.TryGetComponent(out EntityBase entity))
            {
                //entity.EntityData.health = 0;
                //entity.SetActionType(ActionType.Dead);
                return;
            }

            GameObjectPoolManager.Instance.Release(other.gameObject);
        }
    }
}