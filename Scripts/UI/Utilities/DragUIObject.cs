using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Olympus
{
    public class DragUIObject : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        public RectTransform playerIconTransform;

        private Vector3 startPosition = new();

        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float maxScale = 1.5f;
        [SerializeField] private float scrollSpeed = 1.0f;

        private RectTransform rectTransform;
        [SerializeField] private float width;
        [SerializeField] private float height;

        [SerializeField] private Rect mapRect = new();

        public bool mapTest = false;
        public Vector2 testScale;
        public Vector2 testOffset;

        private void Start()
        {
            rectTransform = transform.parent.GetComponent<RectTransform>();
            width = rectTransform.sizeDelta.x;
            height = rectTransform.sizeDelta.y;

            rectTransform = GetComponent<RectTransform>();
            SetMapRect();
        }

        private void Update()
        {
            Vector2 mapScale = Vector2.one;
            Vector2 mapOffset = Vector2.zero;
            float playerRotationYOffset = 90.0f;

            switch(GameDataManager.Instance.stageIslandType)
            {
                case IslandType.Spring: 
                    mapScale.x = 550.0f;
                    mapScale.y = 440.0f;
                    mapOffset.x = -1220.0f;
                    mapOffset.y = 50.0f;
                    playerRotationYOffset = 90.0f;
                    break;
                case IslandType.Summer:
                    mapScale.x = 550.0f;
                    mapScale.y = 440.0f;
                    mapOffset.x = 3750.0f;
                    mapOffset.y = -265.0f;
                    playerRotationYOffset = -110.0f;
                    break;
                case IslandType.Fall:
                    mapScale.x = 550.0f;
                    mapScale.y = 440.0f;
                    mapOffset.x = -50.0f;
                    mapOffset.y = 700.0f;
                    playerRotationYOffset = -110.0f;
                    break;
                case IslandType.Winter:
                    mapScale.x = 550.0f;
                    mapScale.y = 440.0f;
                    mapOffset.x = -2700.0f;
                    mapOffset.y = -1500.0f;
                    playerRotationYOffset = -110.0f;
                    break;
                default:
                    break;
            }

            if(mapTest)
            {
                mapScale = testScale;
                mapOffset = testOffset;
            }

            Vector3 position = PlayerController.Instance.PlayerEntity.transform.position;
            position.x = position.x / mapScale.x * rectTransform.sizeDelta.x + mapOffset.x;
            position.y = position.z / mapScale.y * rectTransform.sizeDelta.y + mapOffset.y;
            position.z = 0.0f;

            position = Quaternion.Euler(0.0f, 0.0f, playerRotationYOffset - 90.0f) * position;
            //position.x += mapOffset.x;
            //position.y += mapOffset.y;
            playerIconTransform.anchoredPosition3D = position;
            playerIconTransform.rotation = Quaternion.Euler(0.0f, 0.0f, playerRotationYOffset - PlayerController.Instance.PlayerEntity.transform.rotation.eulerAngles.y);
        }

        private void SetMapRect()
        {
            mapRect.width = rectTransform.sizeDelta.x * transform.localScale.x;
            mapRect.height = rectTransform.sizeDelta.y * transform.localScale.y;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 position = Input.mousePosition + startPosition;

            if(position.x > 0)
            {
                position.x = 0;
            }
            if(position.y < 0)
            {
                position.y = 0;
            }
            if(position.x < -mapRect.width + width)
            {
                position.x = -mapRect.width + width;
            }
            if(position.y > mapRect.height - height)
            {
                position.y = mapRect.height - height;
            }

            rectTransform.anchoredPosition3D = position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            startPosition = rectTransform.anchoredPosition3D - Input.mousePosition;
        }

        public void OnPointStay()
        {
            float scrollSize = Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime;
            transform.localScale = Vector3.one * Mathf.Clamp(transform.localScale.x + scrollSize, minScale, maxScale);
            SetMapRect();

            Vector3 position = rectTransform.anchoredPosition3D;

            if (position.x > 0)
            {
                position.x = 0;
            }
            if (position.y < 0)
            {
                position.y = 0;
            }
            if (position.x < -mapRect.width + width)
            {
                position.x = -mapRect.width + width;
            }
            if (position.y > mapRect.height - height)
            {
                position.y = mapRect.height - height;
            }

            rectTransform.anchoredPosition3D = position;
        }
    }
}