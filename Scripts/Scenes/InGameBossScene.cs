using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Olympus
{
    public class InGameBossScene : SceneBase
    {
        public override IEnumerator OnStart()
        {
            var async = SceneManager.LoadSceneAsync(nameof(InGameBossScene));
            yield return new WaitUntil(() => { return async.isDone; });
            yield return null;
        }

        public override IEnumerator OnEnd()
        {
            yield return null;
        }
    }
}