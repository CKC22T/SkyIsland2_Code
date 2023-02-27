using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class EnemyHUDManagerUI : UIBase
    {
        public Dictionary<EntityBase, EnemyHUD> enemyHUDs = new();
        [SerializeField] private EnemyHUD enemyHUDPrefab;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
            ClearEnemyHud();
        }

        private void Update()
        {
            foreach(var hud in enemyHUDs.Values)
            {
                hud.SetHealth();
                hud.SetPosition();
            }
        }

        public void RegisterEnemyHud(EntityBase entity)
        {
            if(!enemyHUDs.ContainsKey(entity))
            {
                var hud = GameObjectPoolManager.Instance.CreateGameObject(enemyHUDPrefab);
                hud.transform.parent = transform;
                hud.transform.localScale = Vector3.one;
                hud.SetEnemy(entity);
                enemyHUDs.Add(entity, hud);
            }
        }

        public void UnRegisterEnemyHud(EntityBase entity)
        {
            if (enemyHUDs.ContainsKey(entity))
            {
                GameObjectPoolManager.Instance.Release(enemyHUDs[entity].gameObject);
                enemyHUDs.Remove(entity);
            }
        }

        public void ClearEnemyHud()
        {
            foreach(var hud in enemyHUDs.Values)
            {
                GameObjectPoolManager.Instance.Release(hud.gameObject);
            }
            enemyHUDs.Clear();
        }
    }
}