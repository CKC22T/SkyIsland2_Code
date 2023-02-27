using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Olympus
{
    public class InteractableObject : MonoBehaviour
    {
        public enum InteractType
        {
            Move,
            Attack,
            Misc,
        }

        private void Awake()
        {
        }

        private void OnEnable()
        {
            radius = Mathf.Clamp(radius, 0.5f, radius);
        }

        [SerializeField, TabGroup("Settings")] GameObject[] particlePrefabs;
        public GameObject[] ParticleEffects { get { return particlePrefabs; } }
        [SerializeField, TabGroup("Settings")] float radius;
        [SerializeField, TabGroup("Settings")] InteractType type;
        [SerializeField, TabGroup("Debug")] RenderTexture alphaMap;
        public float Radius { get { return radius; } }
        public InteractType Type { get { return type; } }
        public Vector3 Point { get { return transform.position; } }

        [SerializeField, TabGroup("Optimizations")] bool updateSuppressing = false;
        [SerializeField, TabGroup("Optimizations"), DisableIf("@updateSuppressing == false")] int updateInterval = 144;
        
        public bool UpdateSupressing { get { return updateSuppressing; } }
        public int UpdateInterval { get { return updateInterval; } }

        private void LateUpdate()
        {
            
        }

    }
}