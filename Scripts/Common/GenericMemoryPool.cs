using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class GenericMemoryPool<T> where T : Object
    {
        private T[] objectArray;
        private int capacity;
        public int Capacity
        {
            get { return capacity; }
        }
        public GenericMemoryPool(int initialCapacity)
        {
            capacity = initialCapacity;
            objectArray = new T[initialCapacity];
        }

        // hid default constructor in case of user error.
        // 사용자 실수로 인해 생기는 문제를 막기 위해 기본 생성자 가림
        private GenericMemoryPool() { }

        public void Make()
        {
            for (int i = 0; i < capacity; i++)
            {
                objectArray[i] = new GameObject() as T;
            }

            RemoveAll();
        }
        public void SwitchCondition(T target)
        {
            GameObject instance = target as GameObject;

            instance.SetActive(!instance.activeInHierarchy);
        }

        public T GetElementAt(int index)
        {
            return objectArray[index] as T;
        }

        public T GetElement()
        {
            if (objectArray.Length == 0)
            {
                return null;
            }

            for (int i = 0; i < objectArray.Length; i++)
            {
                if (objectArray[i] != null)
                {
                    return objectArray[i];
                }
            }

            return null;
        }

        public void Remove(T element)
        {
            if (objectArray.Length == 0 || element == null)
            {
                return;
            }

            for (int i = 0; i < objectArray.Length; i++)
            {
                GameObject instance = objectArray[i] as GameObject;
                if (objectArray[i] == element)
                {
                    instance.SetActive(false);
                }
            }
        }

        public void RemoveAll()
        {
            foreach (var i in objectArray)
            {
                GameObject instance = i as GameObject;

                instance.SetActive(false);
            }
        }

        public T Add()
        {
            for (int i = 0; i < objectArray.Length; i++)
            {
                GameObject instance = objectArray[i] as GameObject;
                if (instance.activeInHierarchy == false)
                {
                    instance.SetActive(true);

                    return objectArray[i];
                }
            }

            LogUtil.LogError("Out of bound, no more usable object in the list");

            return null;
        }

    }
}