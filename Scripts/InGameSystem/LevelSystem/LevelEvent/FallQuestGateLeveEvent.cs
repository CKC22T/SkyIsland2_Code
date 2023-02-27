using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class FallQuestGateLeveEvent : LevelEventBase
    {
        public GameObject[] onParts;
        public GameObject[] offParts;
        public Animator wallAnimator;
        public MeshCollider wallCollider;

        public FallQuestLevelEvent fallQuest;

        public override void OnLevelEvent(EntityBase entity)
        {
            if (!gameObject.activeSelf)
            {
                SetParts();
            }
            else
            {
                OpenWall();
            }
        }

        private void SetParts()
        {
            //FallQuestInfoUI ui = UIManager.Instance.GetUI(UIList.FallQuestInfoUI) as FallQuestInfoUI;
            int parts = fallQuest.parts;

            for(int i = 0; i < onParts.Length; ++i)
            {
                onParts[i].SetActive(i < parts);
                offParts[i].SetActive(i >= parts);
            }

            if(parts == 3)
            {
                gameObject.SetActive(true);
            }
        }

        private void OpenWall()
        {
            wallAnimator.Play("WaveWall");
            wallCollider.enabled = false;

            foreach(var part in onParts)
            {
                part.SetActive(false);
            }
            foreach(var part in offParts)
            {
                part.SetActive(false);
            }
        }
    }
}