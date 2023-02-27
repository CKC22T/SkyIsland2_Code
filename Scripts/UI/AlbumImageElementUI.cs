using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Olympus
{
    public class AlbumImageElementUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] AlbumUI root;
        public int index = 0;
        public int globalIndex = 0;
        public void OnPointerClick(PointerEventData eventData)
        {
            root.selectedImageIndex = globalIndex;
        }

        // Start is called before the first frame update
        void Start()
        {
            root = GetComponentInParent<AlbumUI>();

        }

        public void Setup(int elementIndex, int global)
        {
            index = elementIndex;
            globalIndex = global;
        }
    }
}