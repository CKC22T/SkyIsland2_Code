using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.TerrainUtils;
using static UnityEngine.UI.Image;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Olympus
{
    public class DamageEffectLevelEvent : LevelEventBase
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
        //public float colliderTime = 0.1f;
        //public int count = 3;

        public float slowTime = 0.05f;
        public float slowTimeScale = 0.05f;

        public GameObject interactorPrefab;

        public Transform effectView;

        public TerrainData terrainData;
        [SerializeField] Vector2Int[] detailPatches;

        public string soundName = "";

        private InteractionComputeObject interactionComputeObject;

        public override void OnLevelEvent(EntityBase entity)
        {
            if (owner == entity) return;
            if (entity.EntityType == EntityType.Puri) return;
            if (entity.ActionType == ActionType.Dead) return;

            bool isCritical = Random.Range(0.0f, 99.9f) < owner.EntityData.criticalPercent;
            if (isCritical)
            {
                damage *= 2;
            }

            if (entity.GetDamage(damage))
            {
                Vector3 dir = entity.transform.position - owner.transform.position;
                dir.y = 0.0f;

                entity.EntityData.moveDirection = dir.normalized;

                if (entity == BossController.Instance.bossEntity)
                {
                    entity.EntityMove(0.0f, (knockbackPower * 10.0f) / 2, knockbackPower / 2, false);
                    SoundManager.Instance.PlaySound("Homeros_Hit");
                }
                else
                {
                    entity.EntityMove(0.0f, knockbackPower * 10.0f, knockbackPower, false);
                }

                if (owner.EntityType == EntityType.Player)
                {
                    GameManager.Instance.GameSlow(slowTimeScale, slowTime);
                    switch (owner.Weapon.WeaponData.WeaponName)
                    {
                        case "Sword":
                            PlayerCamera.Instance.Shake("BasicSwordAttack");
                            //SoundManager.Instance.PlaySound("Weapon_Sword_Attack");

                            break;
                        case "Spear":
                            PlayerCamera.Instance.Shake("BasicSpearAttack");
                            //SoundManager.Instance.PlaySound("Weapon_Spear_Attack");

                            break;
                        case "Hammer":
                            PlayerCamera.Instance.Shake("BasicHammerAttack");
                            //SoundManager.Instance.PlaySound("Weapon_Hammer_Attack");

                            break;

                    }

                    if (entity.EntityType != EntityType.None)
                    {
                        PuriController.Instance.AttackTarget = entity;
                    }

                }

                PlayHitSound(entity.EntityType);

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
                    Vector3 a = quaternion * Vector3.forward * radius;

                    GameObjectPoolManager.Instance.Release(GameObjectPoolManager.Instance.CreateGameObject(damageParticle.gameObject, entity.CenterPosition.position + a + b, Quaternion.identity), 0.5f);
                    //Instantiate(particle, entity.CenterPosition.position + a + b, Quaternion.identity);
                }
            }
        }

        public void OnEnable()
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

            if (!string.IsNullOrEmpty(soundName))
            {
                SoundManager.Instance.PlaySound(soundName);
            }

            if (interactionComputeObject == null)
            {
                interactionComputeObject = GameObject.FindObjectOfType<InteractionComputeObject>();
            }
            interactionComputeObject.InteractionQueue.Enqueue(this);

        }

        public void EffectOnVegetation()
        {
            if (interactorPrefab != null)
            {
                if (owner != null)
                {
                    InteractionComputeObject system = GameObject.FindObjectOfType<InteractionComputeObject>();

                    if (system.ParticlePrefab != null)
                    {
                        Vector3 origin = new Vector3(transform.position.x, owner.transform.position.y + 3.5f, transform.position.z);
                        // GameObject permenantInteractor = GameObjectPoolManager.Instance.CreateGameObject(interactorPrefab, origin, Quaternion.identity);
                        GameObject permenantInteractor = GameObject.Instantiate(interactorPrefab, origin, Quaternion.identity);

                        system.AddSystemObject(permenantInteractor.GetComponent<InteractableObject>());

                        Destroy(permenantInteractor, 0.5f);
                        // GameObjectPoolManager.Instance.Release(permenantInteractor, 0.5f);
                    }
                }
            }
        }

        public void ParticleIntegration()
        {
            if (interactorPrefab != null)
            {
                if (owner != null)
                {
                    InteractionComputeObject system = GameObject.FindObjectOfType<InteractionComputeObject>();

                    if (system.ParticlePrefab != null)
                    {
                        InteractionComputeObject.VegetationParticleInfo info = system.RequestOverlapTest(0);

                        var particleInstance = GameObjectPoolManager.Instance.CreateGameObject(system.ParticlePrefab, transform.position, Quaternion.identity);
                        particleInstance.transform.localScale = new Vector3(info.scale, info.scale, info.scale);
                        particleInstance.transform.position = info.position + new Vector3(0, 1.0f, 0.0f);

                        GameObjectPoolManager.Instance.Release(particleInstance, 3.0f);
                    }
                }
            }
        }

        public void PlayHitSound(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Player:
                    SoundManager.Instance.PlaySound("Player_Hit");
                    break;
                case EntityType.MagicBoar:
                    SoundManager.Instance.PlayInstance("MagicBore_Hit");
                    break;
                case EntityType.Puri:
                    break;
                case EntityType.Boss:
                    SoundManager.Instance.PlaySound("Homeros_Hit");
                    break;
                case EntityType.Wisp:
                    SoundManager.Instance.PlayInstance("Wisp_Hit");
                    break;
                case EntityType.Spirit:
                    SoundManager.Instance.PlayInstance("Spirit_Hit", true);
                    break;
                case EntityType.FlowerTrap:
                    SoundManager.Instance.PlayInstance("FlowerTrap_Hit", true);
                    break;
                case EntityType.MagicStone:
                    //SoundManager.Instance.PlayInstance("FlowerTrap_Hit", true);
                    break;
                default:
                    break;
            }
        }


    }
}