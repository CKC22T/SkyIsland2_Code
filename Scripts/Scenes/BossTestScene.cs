using Olympus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Olympus
{
    public class BossTestScene : SceneBase
    {
        public override IEnumerator OnEnd()
        {
            yield return null;
        }

        public override IEnumerator OnStart()
        {
            var async = SceneManager.LoadSceneAsync(SceneType.BossTestScene.ToString());
            yield return new WaitUntil(() => { return async.isDone; });
            yield return null;
        }
    }
}