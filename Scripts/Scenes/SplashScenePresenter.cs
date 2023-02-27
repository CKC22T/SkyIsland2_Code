using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class SplashScenePresenter : MonoBehaviour
    {
        public static SplashScenePresenter Instance { get; private set; }

        public GameObject animationEndCheckObject;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(autoChangeScene());

            IEnumerator autoChangeScene()
            {
                yield return new WaitWhile(() => animationEndCheckObject.activeSelf);
                Main.Instance.ChangeScene(SceneType.TitleScene);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}