using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class SingletonBase<T> : MonoBehaviour where T : class
    {
        public static T Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private static readonly Lazy<T> _instance =
            new Lazy<T>(() =>
            {
                T instance = FindObjectOfType(typeof(T)) as T;

                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).ToString());
                    instance = obj.AddComponent(typeof(T)) as T;

#if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                    {
                        DontDestroyOnLoad(obj);
                    }
#else
                DontDestroyOnLoad(obj);
#endif
            }

                return instance;
            });

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public class LocalSingletonBase<T> : MonoBehaviour where T : LocalSingletonBase<T>
    {
        public static T Instance => _instance;

        protected static T _instance = null;

        protected void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if(_instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }

        protected void OnDestroy()
        {
            if(_instance == this)
            {
                _instance = null;
            }
        }
    }
}