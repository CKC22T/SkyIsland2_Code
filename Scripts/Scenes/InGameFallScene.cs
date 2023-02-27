using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Olympus
{
    public class InGameFallScene : SceneBase
    {
        public override IEnumerator OnStart()
        {
            var async = SceneManager.LoadSceneAsync(nameof(InGameFallScene));
            yield return new WaitUntil(() => { return async.isDone; });
            yield return null;
        }

        public override IEnumerator OnEnd()
        {
            yield return null;
        }
    }
}