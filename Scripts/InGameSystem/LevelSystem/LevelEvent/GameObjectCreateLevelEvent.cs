using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class GameObjectCreateLevelEvent : LevelEventBase
    {
        [SerializeField] private GameObject gameObjectPrefab;
        public Transform spawnTransform;
        public bool autoRelease = false;
        public float releaseTime = 1.0f;

        public override void OnLevelEvent(EntityBase entity)
        {
            var obj = GameObjectPoolManager.Instance.CreateGameObject(gameObjectPrefab, spawnTransform.position, spawnTransform.rotation);
            obj.transform.localScale = spawnTransform.lossyScale;
            if(autoRelease)
            {
                GameObjectPoolManager.Instance.Release(obj, releaseTime);
            }
        }
    }
}