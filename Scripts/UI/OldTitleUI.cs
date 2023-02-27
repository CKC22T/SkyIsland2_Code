using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class OldTitleUI : UIBase
    {
        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        public void GameStart()
        {
            UIManager.Hide(UIList.OldTitleUI);
            TestScenePresenter.Instance.GameStart();
        }

        public void Setting()
        {

        }

        public void GameExit()
        {

        }
    }
}