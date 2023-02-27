using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Olympus
{
    public class InGameEndingScene : SceneBase
    {
        public override IEnumerator OnStart()
        {
            var async = SceneManager.LoadSceneAsync(nameof(InGameEndingScene));
            yield return new WaitUntil(() => { return async.isDone; });
            yield return null;
        }

        public override IEnumerator OnEnd()
        {
            yield return null;
        }
    }
}