using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    [System.Serializable]
    public struct DamageEffectInfo
    {
        public DamageEffectLevelEvent damageEffect;
        public float damageEffectTime;
    }

    public class StoneDamageEffectLevelEvent : LevelEventBase
    {
        public EntityBase owner;
        public int damage;

        public ParticleSystem particle;
        public ParticleSystem criticalParticle;
        public float knockbackPower = 100.0f;

        public bool IsAutoDestory = true;
        public float destoryTimer = 0.5f;

        public BoxCollider damageCollider;
        public float startColliderTime = 0.1f;
        public float endColliderTime = 0.4f;

        public DamageEffectInfo[] damageEffectInfos;
        public float damageEffectTimer = 0.0f;
        public int damageEffectInfoIndex = 0;

        public string soundName = "";

        public override void OnLevelEvent(EntityBase entity)
        {
            if (owner == entity) return;
            if (entity.EntityType == EntityType.Puri) return;

            bool isCritical = Random.Range(0.0f, 99.9f) < owner.EntityData.criticalPercent;
            if (isCritical)
            {
                damage *= 2;
            }

            if (entity.GetDamage(damage))
            {
                SoundManager.Instance.PlayInstance("MagicStone_Destroy");
                Vector3 dir = entity.transform.position - owner.transform.position;
                dir.y = 0.0f;
                entity.EntityData.moveDirection = dir.normalized;
                if (entity == BossController.Instance.bossEntity)
                {
                    entity.EntityMove(0.0f, (knockbackPower * 10.0f) / 2, knockbackPower / 2, false);
                }
                else
                {
                    entity.EntityMove(0.0f, knockbackPower * 10.0f, knockbackPower, false);
                }
                //entity.EntityMove(0.0f, knockbackPower * 10.0f, knockbackPower, false);

                //PlayerCamera.Instance.ShakeOnce(0.2f, 0.4f);
                if (owner.EntityType == EntityType.Player)
                {
                    PuriController.Instance.AttackTarget = entity;
                }

                if (owner.EntityType == EntityType.MagicBoar)
                {
                }

                ParticleSystem damageParticle = (isCritical) ? criticalParticle : particle;

                if (damageParticle)
                {
                    float radius = entity.Radius;
                    var camDir = -Camera.main.transform.forward;
                    Vector3 b = new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), Random.Range(-radius, radius));
                    camDir += b;
                    //camDir.y = 0;
                    camDir.Normalize();

                    var quaternion = Quaternion.LookRotation(camDir);
                    Vector3 a = quaternion * Vector3.forward * radius * 2.0f;

                    GameObjectPoolManager.Instance.Release(GameObjectPoolManager.Instance.CreateGameObject(damageParticle.gameObject, entity.CenterPosition.position + a + b, Quaternion.identity), 0.5f);
                }
            }
        }

        private void OnEnable()
        {
            if (IsAutoDestory)
            {
                GameObjectPoolManager.Instance.Release(gameObject, destoryTimer);
            }

            if (damageCollider)
            {
                StartCoroutine(colliderTwinkle());

                IEnumerator colliderTwinkle()
                {
                    damageCollider.enabled = false;
                    yield return new WaitForSeconds(startColliderTime);
                    damageCollider.enabled = true;
                    yield return new WaitForSeconds(endColliderTime - startColliderTime);
                    damageCollider.enabled = false;
                }
            }

            if(!string.IsNullOrEmpty(soundName))
            {
                SoundManager.Instance.PlaySound(soundName);
            }

            foreach (var damageEffect in damageEffectInfos)
            {
                damageEffect.damageEffect.gameObject.SetActive(false);
            }

            damageEffectInfoIndex = 0;
        }

        private void Update()
        {
            if (damageEffectInfoIndex < damageEffectInfos.Length)
            {
                damageEffectTimer += Time.deltaTime;
                if (damageEffectInfos[damageEffectInfoIndex].damageEffectTime < damageEffectTimer)
                {
                    var effect = damageEffectInfos[damageEffectInfoIndex].damageEffect;
                    effect.damage = damage;
                    effect.owner = owner;
                    effect.gameObject.SetActive(true);

                    damageEffectTimer -= damageEffectInfos[damageEffectInfoIndex].damageEffectTime;
                    ++damageEffectInfoIndex;
                }
            }
        }
    }
}