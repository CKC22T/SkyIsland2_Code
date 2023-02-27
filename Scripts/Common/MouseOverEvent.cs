using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Olympus
{
    public class MouseOverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Sirenix.OdinInspector.Title("UI Mouse Enter/Over/Exit Callback Events")]
        public UnityEvent pointEnterEvent;
        public UnityEvent pointOverEvent;
        public UnityEvent pointExitEvent;

        public bool isOver = true;

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointEnterEvent.Invoke();
            isOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointExitEvent.Invoke();
            isOver = false;
        }

        public void Update()
        {
            if(isOver)
            {
                pointOverEvent.Invoke();
            }
        }
    }
}