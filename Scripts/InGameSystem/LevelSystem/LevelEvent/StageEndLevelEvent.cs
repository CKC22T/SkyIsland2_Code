using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class StageEndLevelEvent : LevelEventBase
    {
        public IslandType currentIslandType;
        public SceneType nextScene;

        public bool isShowStageEndUI = true;

        public override void OnLevelEvent(EntityBase entity)
        {
            GameDataManager.Instance.SetScore();
            if (isShowStageEndUI)
            {
                UIBase baseElement = UIManager.Show(UIList.StageMoveUI);
                StageMoveUI element = baseElement as StageMoveUI;
                element.SetStageMove(nextScene);
            }
            else
            {
                Main.Instance.ChangeScene(nextScene);
            }
        }
    }
}