using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class PauseUI : UIBase
    {
        public GameObject really;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            really.SetActive(false);
            GameManager.Instance.GamePause(0.0f);
            animationTimer.UseUnscaled = true;
        }

        public override void Hide(UnityAction callback = null)
        {
            Animate(true, () => {
                base.Hide(callback);
                GameManager.Instance.GameContinue();
            });
        }

        public void Continue()
        {
            UIManager.Hide(UIList.PauseUI);
        }

        public void Setting()
        {
            UIManager.Show(UIList.SettingUI);
        }

        public void GotoLobby()
        {
            really.SetActive(true);
        }

        public void GotoLobbyYes()
        {
            Main.Instance.ChangeScene(SceneType.TitleScene);
        }

        public void GotoLobbyNo()
        {
            really.SetActive(false);
        }
    }
}