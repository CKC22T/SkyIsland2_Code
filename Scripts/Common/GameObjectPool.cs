using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class GameObjectPool
    {
        public int Count => objectStack.Count;
        private Stack<GameObject> objectStack = new();
        private GameObject referenceInstance;
        private Transform transform;

        public GameObjectPool(GameObject referenceInstance, Transform transform)
        {
            this.referenceInstance = referenceInstance;
            this.transform = transform;
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            if (objectStack.TryPop(out GameObject instance))
            {
                instance.transform.position = position;
                instance.transform.rotation = rotation;
                instance.SetActive(true);
                return instance;
            }

            var go = Object.Instantiate(referenceInstance, position, rotation, transform);
            go.SetActive(true);
            return go;
        }

        public void Release(GameObject instance)
        {
            instance.SetActive(false);
            instance.transform.parent = transform;
            objectStack.Push(instance);
        }

        public void Clear()
        {
            while (objectStack.Count > 0)
            {
                var instance = objectStack.Pop();
                Object.Destroy(instance);
            }
        }

        public GameObject[] GetPoolingObjects()
        {
            return objectStack.ToArray();
        }
    }
}