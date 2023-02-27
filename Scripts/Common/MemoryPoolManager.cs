using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Windows;

namespace Olympus
{
    public class MemoryPoolManager : SingletonBase<MemoryPoolManager>
    {
        private Dictionary<int, GenericMemoryPool<GameObject>> objectPools = new();
        
        public void Initialize()
        {
            var resourceList = Resources.LoadAll("Resources/Temp/");
            for (int i = 0; i < resourceList.Length; i++)
            {
                string name = resourceList[i].name.Substring(resourceList[i].name.LastIndexOf('/') + 1);
                name = name.Substring(0, name.IndexOf('.'));
                int hash = name.GetHashCode();
                if (Instance.objectPools.ContainsKey(hash) == false)
                {
                    Instance.objectPools.Add(name.GetHashCode(), new GenericMemoryPool<GameObject>(256));
                }
            }
        }

        //public GameObject Add<T>()
        //{
        //    GenericMemoryPool<GameObject> pool;
        //    LogUtil.Assert(objectPools.TryGetValue(nameof(T).GetHashCode(), out pool) != false);

        //    return pool.Add();
        //}

        //public void Remove<T>(GameObject target)
        //{
        //    GenericMemoryPool<GameObject> pool;
        //    LogUtil.Assert(objectPools.TryGetValue(nameof(T).GetHashCode(), out pool) != false);

        //    pool.Remove(target);
        //}

        //public GameObject GetObject<T>()
        //{
        //    GenericMemoryPool<GameObject> pool;
        //    LogUtil.Assert(objectPools.TryGetValue(nameof(T).GetHashCode(), out pool) != false);

        //    return pool.GetElement();
        //}

        //public C GetComponentFromValidObject<T, C>()
        //{
        //    GenericMemoryPool<GameObject> pool;
        //    LogUtil.Assert(objectPools.TryGetValue(nameof(T).GetHashCode(), out pool) != false);

        //    return pool.GetElement().GetComponent<C>();
        //}
    }
}