using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class GameObjectPoolManager : SingletonBase<GameObjectPoolManager>
    {
        private Dictionary<GameObject, GameObjectPool> gameObjectPoolTable = new();
        private Dictionary<GameObject, GameObjectPool> gameObjectInstancePoolTable = new();
        private Dictionary<GameObject, Coroutine> gameObjectInstanceDestroyCoroutineTable = new();

        public GameObjectPool GetGameObjectPool (GameObject prefab)
        {
            if(gameObjectPoolTable.TryGetValue(prefab, out var pool))
            {
                return pool;
            }
            return null;
        }

        public T CreateGameObject<T>(T prefab) where T : MonoBehaviour
        {
            return CreateGameObject(prefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<T>();
        }

        public T CreateGameObject<T>(T prefab, Vector3 position, Quaternion rotation) where T : MonoBehaviour
        {
            return CreateGameObject(prefab.gameObject, position, rotation).GetComponent<T>();
        }

        public GameObject CreateGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject poolInstance;
            GameObjectPool monoPool;

            if (!gameObjectPoolTable.TryGetValue(prefab, out monoPool))
            {
                monoPool = new GameObjectPool(prefab, transform);
                gameObjectPoolTable.Add(prefab, monoPool);
            }

            poolInstance = gameObjectPoolTable[prefab].Get(position, rotation);
            gameObjectInstancePoolTable.Add(poolInstance, monoPool);

            return poolInstance;
        }

        public void Release(GameObject instance)
        {
            if (gameObjectInstanceDestroyCoroutineTable.ContainsKey(instance))
            {
                StopCoroutine(gameObjectInstanceDestroyCoroutineTable[instance]);
                gameObjectInstanceDestroyCoroutineTable.Remove(instance);
            }

            if (gameObjectInstancePoolTable.ContainsKey(instance))
            {
                gameObjectInstancePoolTable[instance].Release(instance);
                gameObjectInstancePoolTable.Remove(instance);
                return;
            }

            if (instance != null)
            {
                Destroy(instance);
            }
        }

        public void Release(GameObject instance, float releaseTime)
        {
            if (!gameObjectInstanceDestroyCoroutineTable.ContainsKey(instance))
            {
                gameObjectInstanceDestroyCoroutineTable.Add(instance, StartCoroutine(ReleaseRoutine(instance, releaseTime)));
            }
        }

        private IEnumerator ReleaseRoutine(GameObject instance, float releaseTime)
        {
            yield return new WaitForSeconds(releaseTime);

            if (gameObjectInstanceDestroyCoroutineTable.ContainsKey(instance))
            {
                gameObjectInstanceDestroyCoroutineTable.Remove(instance);
            }

            if (gameObjectInstancePoolTable.ContainsKey(instance))
            {
                gameObjectInstancePoolTable[instance].Release(instance);
                gameObjectInstancePoolTable.Remove(instance);
                yield break;
            }
            
            if (instance != null)
            {
                Destroy(instance);
            }
        }

        public void Release()
        {
            foreach(var instance in gameObjectInstancePoolTable.Keys)
            {
                if (gameObjectInstanceDestroyCoroutineTable.ContainsKey(instance))
                {
                    StopCoroutine(gameObjectInstanceDestroyCoroutineTable[instance]);
                    gameObjectInstanceDestroyCoroutineTable.Remove(instance);
                }

                if (gameObjectInstancePoolTable.ContainsKey(instance))
                {
                    gameObjectInstancePoolTable[instance].Release(instance);
                }
            }
            gameObjectInstancePoolTable.Clear();
        }
    }
}