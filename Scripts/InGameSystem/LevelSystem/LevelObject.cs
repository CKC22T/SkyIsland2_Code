using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class LevelObject : MonoBehaviour
    {
        [field: Sirenix.OdinInspector.Title("Level Object Option")]
        [field: SerializeField] public bool IsActive { get; set; } = false;
        [field: SerializeField] public bool IsRepeat { get; set; } = false;

        [field: Sirenix.OdinInspector.Title("Level Event List")]
        [field: SerializeField] public List<LevelEventBase> levelEvents = new();

        private InteractiveUI interactiveUI;
        public bool isTalk = false;
        public Vector3 offset;
        public Vector3 interactiveUIPosition => transform.position + offset;

        private void OnTriggerEnter(Collider other)
        {
            if (IsActive)
            {
                if (other.TryGetComponent(out EntityBase entity))
                {
                    //InteractionUIPrefab.SetActive(true);
                    //UIManager.Show(UIList.InteractiveUI);
                    interactiveUI.ConnectInteractiveUI(this);
                }
            }
        }

        //private void OnTriggerStay(Collider other)
        //{
        //    if (IsActive)
        //    {
        //        if (other.TryGetComponent(out EntityBase entity))
        //        {
        //            //상호작용 키
        //            if (Input.GetKeyDown(KeyCode.F))
        //            {
        //                foreach (var e in levelEvents)
        //                {
        //                    e.OnLevelEvent(entity);
        //                }
        //                IsActive = IsRepeat;
        //                UIManager.Hide(UIList.InteractiveUI);
        //            }
        //        }
        //    }
        //}

        public void InteractLevelEvent()
        {
            foreach (var e in levelEvents)
            {
                e.OnLevelEvent(PlayerController.Instance.PlayerEntity);
            }
            IsActive = IsRepeat;
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsActive)
            {
                if (other.TryGetComponent(out EntityBase entity))
                {
                    //InteractionUIPrefab.SetActive(false);
                    //UIManager.Hide(UIList.InteractiveUI);
                    interactiveUI.DisconnectInteractiveUI(this);
                }
            }
            //UIManager.Hide(UIList.UpgradeUI);
        }

        private void OnEnable()
        {
            interactiveUI = UIManager.Instance.GetUI(UIList.InteractiveUI) as InteractiveUI;
        }

        private void OnDisable()
        {
            //UIManager.Hide(UIList.InteractiveUI);
            interactiveUI.DisconnectInteractiveUI(this);
        }

        private void OnDestroy()
        {
            //UIManager.Hide<InteractiveUI>(UIList.InteractiveUI);
            interactiveUI.DisconnectInteractiveUI(this);
        }
    }
}