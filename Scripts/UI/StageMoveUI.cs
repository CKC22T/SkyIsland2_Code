using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class StageMoveUI : UIBase
    {
        [SerializeField] private Animator stageAnimator;
        [SerializeField] private AnimationEventListener animationEventListener;

        [SerializeField] private SceneType nextScene;

        public override void Show(UnityAction callback = null)
        {
            PlayerController.Instance.InputLock(LockType.FromGUI);
            base.Show(callback);
            animationEventListener.OnAnimationEventAction = AnimationEventAction;
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
            animationEventListener.OnAnimationEventAction = null;
        }

        public void SetStageMove(SceneType sceneType)
        {
            UIManager.Show(Id);
            //GameDataManager.Instance.SetScore();
            nextScene = sceneType;
            stageAnimator.Play("Go" + GameData.SceneTypeToIslaneType(nextScene).ToString());
        }

        public void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            switch(eventId)
            {
                case AnimationEventTriggerType.AnimationEnd:
                    Main.Instance.ChangeScene(nextScene);
                    break;
            }
        }
    }
}