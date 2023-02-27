using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    [System.Serializable]
    public class SerializablePair<T1, T2>
    {
        [SerializeField]
        private T1 left;
        public T1 Left { get { return left; } }
        [SerializeField]
        private T2 right;
        public T2 Right { get { return right; } }

    }
}