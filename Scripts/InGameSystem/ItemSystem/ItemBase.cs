using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ItemBase : MonoBehaviour
    {
        [field: Sirenix.OdinInspector.Title("Item Param")]
        [field: SerializeField] public ItemData ItemData { get; private set; }
        [field: SerializeField] private EntityBase followEntity = null;
        [field: SerializeField] private EntityType followEntityType = EntityType.None;
        [field: SerializeField] private float moveSpeed = 1.0f;
        [field: SerializeField] private float moveTimer = 0.0f;

        [field: SerializeField] private float dropTime = 1.0f;
        [field: SerializeField] private float dropTimer = 0.0f;
        [field: SerializeField] private AnimationCurve dropAnimationCurve;
        [field: SerializeField] private float dropHeight = 1.0f;
        [field: SerializeField] private float dropDistance = 3.0f;
        [field: SerializeField] private Vector3 dropDir;
        [field: SerializeField] private Vector3 dropStartPosition;
        [field: SerializeField] private Vector3 dropEndPosition;

        private void OnEnable()
        {
            dropTimer = 0.0f;
            dropDir = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)).normalized;
            dropStartPosition = transform.position;
            dropEndPosition = transform.position + dropDir * dropDistance;
        }

        private void OnDisable()
        {
        }

        private void Update()
        {
            if(dropTimer < dropTime)
            {
                dropTimer += Time.deltaTime;

                float t = dropTimer / dropTime;

                Vector3 movePosition = Vector3.Lerp(dropStartPosition, dropEndPosition, t);
                movePosition.y = Mathf.Lerp(dropStartPosition.y, dropStartPosition.y + dropHeight, dropAnimationCurve.Evaluate(t));

                transform.position = movePosition;

                if(dropTimer > dropTime)
                {
                    transform.position = dropEndPosition;
                    moveTimer = 0.0f;
                    if(followEntityType == EntityType.Player)
                    {
                        //followEntity = PlayerController.Instance.PlayerEntity;
                    }
                    if(followEntityType == EntityType.Puri)
                    {
                        //followEntity = PuriController.Instance.PuriEntity;
                    }
                }
            }

            else if (followEntity)
            {
                float distance = Vector3.Distance(transform.position, followEntity.CenterPosition.position);
                if (distance < moveSpeed * 0.1f)
                {
                    switch (ItemData.ItemType)
                    {
                        case ItemType.HP:
                            SoundManager.Instance.PlaySound("Player_Healing");
                            PlayerController.Instance.PlayerEntity.GetHeal(2);
                            break;
                        case ItemType.Star:
                            PlayerController.Instance.PlayerEntity.GetUltimateGauge(0.25f);
                            //GameManager.Instance.GetStar(); 
                            break;
                    }
                    GameObjectPoolManager.Instance.Release(gameObject);
                    return;
                }

                moveTimer += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(dropEndPosition, followEntity.CenterPosition.position, moveTimer);

                //timer += Time.deltaTime;
                //if (timer < moveTimer)
                //{
                //    transform.position += moveDir * moveSpeed * (moveTimer - timer) * Time.deltaTime;
                //}
                //else
                //{
                //    transform.position = Vector3.Lerp(transform.position, followEntity.CenterPosition.position, (timer - moveTimer) * Time.deltaTime * moveSpeed);
                //}
                //transform.position += moveDir2 * moveSpeed * Time.deltaTime * 0.5f;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<EntityBase>(out var entity))
            {
                if (followEntityType == entity.EntityType)
                {
                    followEntity = entity;
                    //switch (ItemData.ItemType)
                    //{
                    //    case ItemType.HP:
                    //        followEntity = PlayerController.Instance.PlayerEntity;
                    //        break;
                    //    case ItemType.Star:
                    //        followEntity = PuriController.Instance.PuriEntity;
                    //        break;
                    //}
                }
            }
        }
    }
}