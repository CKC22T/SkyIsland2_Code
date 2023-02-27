using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class InteractiveUI : UIBase
    {
        public Transform interactUITransform;

        public GameObject interactUIObject;
        public GameObject talkUIObject;

        public LevelObject targetLevelObject;
        [SerializeField] private List<LevelObject> interactLevelObjects = new();

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
            SetTargetLevelObject(null);
        }

        public void ConnectInteractiveUI(LevelObject levelObject)
        {
            interactLevelObjects.Add(levelObject);
            SetTargetLevelObject(levelObject);
            UIManager.Show(Id);
        }

        private void SetTargetLevelObject(LevelObject levelObject)
        {
            targetLevelObject = levelObject;

            if(targetLevelObject == null)
            {
                return;
            }

            if(levelObject.isTalk)
            {
                interactUIObject.SetActive(false);
                talkUIObject.SetActive(true);
            }
            else
            {
                interactUIObject.SetActive(true);
                talkUIObject.SetActive(false);
            }
        }

        public void DisconnectInteractiveUI(LevelObject levelObject)
        {
            if (interactLevelObjects.Remove(levelObject))
            {
                if (interactLevelObjects.Count == 0)
                {
                    UIManager.Hide(Id);
                }
                else if (targetLevelObject == levelObject)
                {
                    SetTargetLevelObject(interactLevelObjects[interactLevelObjects.Count - 1]);
                    //targetLevelObject = interactLevelObjects[interactLevelObjects.Count - 1];
                }
            }
        }

        private void Update()
        {
            if (targetLevelObject)
            {
                interactUITransform.position = Camera.main.WorldToScreenPoint(targetLevelObject.interactiveUIPosition);

                if (Input.GetKeyDown(KeyCode.F))
                {
                    targetLevelObject.InteractLevelEvent();
                    //SoundManager.Instance.PlaySound("UI_InGame(039)");
                    DisconnectInteractiveUI(targetLevelObject);
                    //UIManager.Hide(Id);
                }
            }
        }
    }
}