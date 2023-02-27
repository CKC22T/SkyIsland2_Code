using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class UICamera : SingletonBase<UICamera>
    {
        public Camera targetCamera;

        private void Awake()
        {
            base.Awake();
            targetCamera = GetComponent<Camera>();
        }
    }
}