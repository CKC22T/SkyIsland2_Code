using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ArrowLevelEvent : LevelEventBase
    {
        public EntityBase owner;
        public int damage;
        public float moveSpeed = 1.0f;

        public ParticleSystem particle;
        public ParticleSystem criticalParticle;
        public float knockbackPower = 100.0f;

        public float destroyTime = 0.5f;
        public bool isPenetrate = false;

        public LayerMask hitTargetLayer;

        public List<EntityBase> hitTargets = new();

        public string soundName = "";

        public override void OnLevelEvent(EntityBase entity)
        {
            Attack(entity);
        }

        public void OnEnable()
        {
            GameObjectPoolManager.Instance.Release(gameObject, destroyTime);
            hitTargets.Clear();

            if (!string.IsNullOrEmpty(soundName))
            {
                SoundManager.Instance.PlaySound(soundName);
            }
        }

        private void Update()
        {
            if (Physics.SphereCast(transform.position - transform.forward * moveSpeed * Time.deltaTime, 1.0f, transform.forward, out var hit, moveSpeed * Time.deltaTime * 2, hitTargetLayer))
            {
                if (hit.transform.TryGetComponent(out EntityBase entity))
                {
                    Attack(entity);
                }
            }
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }

        private void Attack(EntityBase entity)
        {
            if (owner == entity) return;
            if (entity.EntityType == EntityType.Puri) return;

            if (owner.EntityType == entity.EntityType) return;
            if (owner.EntityType == EntityType.Player)
            {
                if (entity.EntityType == EntityType.Player) return;
            }
            else if (entity.EntityType != EntityType.Player)
            {
                return;
            }

            bool isCritical = Random.Range(0.0f, 99.9f) < owner.EntityData.criticalPercent;
            if (isCritical)
            {
                damage *= 2;
            }

            if(hitTargets.Contains(entity))
            {
                return;
            }

            if (entity.GetDamage(damage))
            {
                hitTargets.Add(entity);

                entity.EntityData.moveDirection = transform.forward;
                if (entity == BossController.Instance.bossEntity)
                {
                    entity.EntityMove(0.0f, (knockbackPower * 10.0f) / 2, knockbackPower / 2, false);
                }
                else
                {
                    entity.EntityMove(0.0f, knockbackPower * 10.0f, knockbackPower, false);
                }

                if (owner.EntityType == EntityType.Player)
                {
                    PuriController.Instance.AttackTarget = entity;
                    SoundManager.Instance.PlayInstance("Weapon_Arrow_Attack");
                    //GameManager.Instance.GameSlow();
                }

                ParticleSystem damageParticle = (isCritical) ? criticalParticle : particle;

                if (damageParticle)
                {
                    GameObjectPoolManager.Instance.Release(GameObjectPoolManager.Instance.CreateGameObject(damageParticle.gameObject, transform.position, Quaternion.LookRotation(-Camera.main.transform.forward.normalized)), 0.5f);
                }
            }

            if (!isPenetrate)
            {
                GameObjectPoolManager.Instance.Release(gameObject);
            }
        }
    }
}