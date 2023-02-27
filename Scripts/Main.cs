using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Olympus
{
    public class Main : MonoBehaviour
    {
        public enum VSyncType
        {
            NEVER,
            EVERY_VERTICAL_BLANK,
            EVERY_SECOND_VERTICAL_BLANK,
        }

        [field: Title("Debug")]
        [field: SerializeField] public SceneType CurrentSceneType { get; private set; } = SceneType.None;

        [field: Sirenix.OdinInspector.Title("Start Condition")]
        [field: SerializeField] public SceneType LoadDirectScene { get; private set; }

        private bool FrameRateLock = false;
        [field: SerializeField, DisableIf("@this.FrameRateLock == true || VSyncOption != VSyncType.NEVER")] public int TargetFrameRate { get; private set; }
        [field: SerializeField, DisableIf("@this.FrameRateLock == true")] public VSyncType VSyncOption { get; private set; }
        public void Initialize()
        {
            StartCoroutine(MainSystemInitialize());
        }

        private IEnumerator MainSystemInitialize()
        {
            var asyncStartup = SceneManager.LoadSceneAsync(SceneType.Startup.ToString(), LoadSceneMode.Single);
            yield return new WaitUntil(() => { return asyncStartup.isDone; });

            //Singleton Initialize
            UIManager.Instance.Initialize();
            WorldUIRoot.Instance.Initialize();
            MemoryPoolManager.Instance.Initialize();
            ResolutionManager.Instance.Initialize();


            Application.targetFrameRate = TargetFrameRate;
            QualitySettings.vSyncCount = (int)VSyncOption;
            FrameRateLock = true;

            Cursor.visible = false;

            ChangeScene(LoadDirectScene);
        }

        [Button("Scene Change")]
        public void ChangeScene(SceneType sceneType, Action sceneLoadCallback = null)
        {
            //if (CurrentSceneType == sceneType)
            //    return;

            switch (sceneType)
            {
                case SceneType.TestScene:
                    ChangeScene<TestScene>(sceneType, sceneLoadCallback);
                    break;

                case SceneType.AITestScene:
                    ChangeScene<AITestScene>(sceneType, sceneLoadCallback);
                    break;

                case SceneType.BossTestScene:
                    ChangeScene<BossTestScene>(sceneType, sceneLoadCallback);
                    break;

                case SceneType.TitleScene:
                    ChangeScene<TitleScene>(sceneType, sceneLoadCallback);
                    break;

                case SceneType.InGameSpringScene:
                    ChangeScene<InGameSpringScene>(sceneType, sceneLoadCallback);
                    break;

                case SceneType.InGameSummerScene:
                    ChangeScene<InGameSummerScene>(sceneType, sceneLoadCallback);
                    break;

                case SceneType.InGameFallScene:
                    ChangeScene<InGameFallScene>(sceneType, sceneLoadCallback);
                    break;

                case SceneType.InGameWinterScene:
                    ChangeScene<InGameWinterScene>(sceneType, sceneLoadCallback);
                    break;

                case SceneType.InGameBossScene:
                    ChangeScene<InGameBossScene>(sceneType, sceneLoadCallback);
                    break;

                case SceneType.InGameEndingScene:
                    ChangeScene<InGameEndingScene>(sceneType, sceneLoadCallback);
                    break;

                case SceneType.SplashScene:
                    ChangeScene<SplashScene>(sceneType, sceneLoadCallback);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public static Main Instance { get; private set; } = null;
        public bool IsOnProgressSceneChange { get; private set; }
        private SceneBase sceneController;
        public event Action OnSceneChangeStart;
        public event Action OnSceneChangeEnd;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Start()
        {
            Initialize();
        }

        private void ChangeScene<T>(SceneType sceneType, Action sceneLoadedCallback = null) where T : SceneBase
        {
            if (IsOnProgressSceneChange)
            {
                return;
            }

            StartCoroutine(ChangeSceneAsync<T>(sceneType, sceneLoadedCallback));
        }

        private IEnumerator ChangeSceneAsync<T>(SceneType sceneType, Action sceneLoadedCallback = null) where T : SceneBase
        {
            IsOnProgressSceneChange = true;

            UIManager.HideAll();
            GameObjectPoolManager.Instance.Release();
            //TODO: LoadingUI Show
            if (sceneType != SceneType.SplashScene)
            {
                UIManager.Show(UIList.LoadingUI);
            }

            if (sceneController)
            {
                OnSceneChangeStart?.Invoke();

                yield return StartCoroutine(sceneController.OnEnd());
                Destroy(sceneController.gameObject);
            }

            var async = SceneManager.LoadSceneAsync(SceneType.Empty.ToString());
            yield return new WaitUntil(() => { return async.isDone; });

            GameObject sceneGO = new GameObject(typeof(T).Name);
            sceneGO.transform.parent = transform;
            sceneController = sceneGO.AddComponent<T>();
            CurrentSceneType = sceneType;

            //yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(sceneController.OnStart());

            //TODO: LoadingUI Hide
            UIManager.Hide(UIList.LoadingUI);

            IsOnProgressSceneChange = false;
            sceneLoadedCallback?.Invoke();
            OnSceneChangeEnd?.Invoke();
        }
    }
}