using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Olympus
{
    public class GameManager : SingletonBase<GameManager>
    {
        [SerializeField] private List<EntityBase> sceneEntityContainer = new();
        [SerializeField] private SerializableDictionary<int, IEntityController> controllerBuffer = new();
        [SerializeField] private List<QuestData> questDatas = new();
        [SerializeField] private List<ScenarioData> scenarioDatas = new();
        [SerializeField] private ScriptTrigger scriptTrigger;
        [SerializeField] public List<Texture2D> AlbumBuffer { get; private set; } = new();

        private EntityBase playerEntity;
        private PlayerCamera playerCameraInstance;
        private PhotoMode photoModeInstance;

        public float gameTimer = 0.0f;

        public List<EntityBase> SceneEntityContainer {
            get { return sceneEntityContainer; }
        }
        public Dictionary<int, IEntityController> ControllerBuffer {
            get { return controllerBuffer; }
        }

        static public void Initialize()
        {
            Instance.playerCameraInstance = Camera.main.GetComponent<PlayerCamera>();
            //Instance.photoModeInstance = Camera.main.GetComponent<PhotoMode>();

            //Instance.photoModeInstance.enabled = false;

            if(Camera.main.TryGetComponent<PhotoMode>(out Instance.photoModeInstance))
            {
                Instance.photoModeInstance.enabled = false;
            }


            Instance.playerEntity = PlayerController.Instance.PlayerEntity;
        }

        public void AddPhoto(Texture2D image)
        {
            if (AlbumBuffer.Contains(image) == false)
            {
                AlbumBuffer.Add(image);
            }
        }

        public void SetPlayerPhotoMode(bool flag)
        {
            if (flag == true)
            {
                playerEntity.EntityData.moveDirection = new Vector3(0, 0, 0);
                playerEntity.ChangeEntityController(NullController.Instance);
            }
            else
            {
                playerEntity.ChangeEntityController(PlayerController.Instance);
            }

            playerCameraInstance.enabled = !flag;
            if(photoModeInstance != null)
            {
                photoModeInstance.enabled = flag;
            }
        }

        private void Update()
        {
            gameTimer += Time.deltaTime;
            if (photoModeInstance != null)
            {
                if (photoModeInstance.enabled == true)
                {
                    if (Input.GetMouseButtonUp(1) == true)
                    {
                        SetPlayerPhotoMode(false);
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.C) && PlayerController.Instance.IsControlLocked == false)
                    {
                        SetPlayerPhotoMode(true);
                    }
                }
            }
        }

        public void ConnectScriptTrigger(ScriptTrigger trigger)
        {
            scriptTrigger = trigger;
        }

        public void DisconnectScriptTrigger()
        {
            scriptTrigger = null;
        }

        public void StartQuest(int code)
        {
            QuestData quest = questDatas.Find(data => data.questCode == code);
            if (quest)
            {
                var questUI = UIManager.Instance.GetUI(UIList.QuestInfoUI) as QuestInfoUI;

                if (!questUI.isActiveAndEnabled)
                {
                    questUI.Show();
                }
                questUI.SetQuestData(quest);
            }
            else
            {
                LogUtil.LogError($"Not Found Quest :: ScenarioCode [{code}]");
            }
        }

        public void EndQuest(int code)
        {
            scriptTrigger?.OnTrigger(code);
        }

        public void StartScenario(int code)
        {
            //ScenarioData scenario = null;
            //foreach (var i in scenarioDatas)
            //{
            //    if (i == null)
            //    {
            //        continue;
            //    }
            //    if (i.scenarioCode == code)
            //    {
            //        scenario = i;
            //        break;
            //    }
            //}

            ScenarioData scenario = scenarioDatas.Find(data => data?.scenarioCode == code);
            if (scenario)
            {
                if (scenario.npcCode == EntityType.None)
                {
                    var ui = UIManager.Show(UIList.TutorialUI);
                    TutorialUI tutorial = ui as TutorialUI;
                    tutorial.ShowScript(scenario);
                }
                else
                {

                    EntityBase target = sceneEntityContainer.Find(entity => entity.EntityType == scenario.npcCode);
                    var ui = UIManager.Show(UIList.DialogUI);
                    DialogUI dialog = ui as DialogUI;
                    dialog.SetDialogTarget(target);
                    dialog.SetScenario(scenario);
                }
            }
            else
            {
                LogUtil.LogError($"Not Found Quest :: ScenarioCode [{code}]");
            }
        }

        public void EndScenario(int code)
        {
            //ScenarioData scenario = null;
            //foreach (var i in scenarioDatas)
            //{
            //    if (i == null)
            //    {
            //        continue;
            //    }
            //    if (i.scenarioCode == code)
            //    {
            //        scenario = i;
            //        break;
            //    }
            //}

            scriptTrigger?.OnTrigger(code);
            ScenarioData scenario = scenarioDatas.Find(data => data?.scenarioCode == code);
            if (scenario)
            {
                if (scenario.npcCode == EntityType.None)
                {
                    var ui = UIManager.Hide(UIList.TutorialUI);
                }
                else if (scenario.nextScenarioCode > 0)
                {
                    StartScenario(scenario.nextScenarioCode);
                }
            }
            else
            {
                LogUtil.LogError($"Not Found Scenario :: ScenarioCode [{code}]");
            }
        }


        [field: SerializeField] public bool IsGameStop { get; set; } = false;
        [field: SerializeField] public Vector3 CheckPoint { get; set; } = Vector3.zero;
        [field: SerializeField] public int StarCount { get; private set; } = 0;
        [field: SerializeField] private Coroutine hitRoutine = null;

        [ShowInInspector] public float gameTimeScale => Time.timeScale;
        private Coroutine gamePauseRoutine = null;

        [field: SerializeField] public float slowTimeScale = 0.1f;
        [field: SerializeField] public float slowTime = 0.2f;

        [field: SerializeField] public AnimationCurve puriXZScale;
        [field: SerializeField] public AnimationCurve puriYScale;
        [field: SerializeField] public float scaleSize;

        [Button]
        public void GetStar(int count = 1)
        {
            StarCount += count;
            SettingStar();

        }

        public void UseStar(int count)
        {
            StarCount -= count;
            SettingStar();
        }

        public void SettingStar()
        {
            ItemSlotUI ui = UIManager.Instance.GetUI(UIList.ItemSlotUI) as ItemSlotUI;
            ui.SetItemCount(StarCount);
            //PuriController.Instance.PuriEntity.EntityModel.localScale = Vector3.one * (1 + 0.1f * StarCount);

            Transform puriTransform = PuriController.Instance.PuriEntity.transform;

            if (StarCount < GameData.UPGRADE_STAR[0])
            {
                puriTransform.localScale = Vector3.one;
            }
            else
            {
                float t = (float)(StarCount - GameData.UPGRADE_STAR[0]) / (GameData.UPGRADE_STAR[2] - GameData.UPGRADE_STAR[0]);
                float xz = Mathf.Lerp(1.0f, scaleSize, puriXZScale.Evaluate(t));
                float y = Mathf.Lerp(1.0f, scaleSize, puriYScale.Evaluate(t));
                puriTransform.localScale = new Vector3(xz, y, xz);
            }
        }

        public void GamePause(float pauseDelay)
        {
            IsGameStop = true;

            if (gamePauseRoutine != null)
            {
                StopCoroutine(gamePauseRoutine);
                gamePauseRoutine = null;
            }
            gamePauseRoutine = StartCoroutine(gameStop());

            IEnumerator gameStop()
            {
                yield return new WaitForSeconds(pauseDelay);
                Time.timeScale = 0.0f;
            }
        }

        public void GameContinue()
        {
            IsGameStop = false;

            if (gamePauseRoutine != null)
            {
                StopCoroutine(gamePauseRoutine);
                gamePauseRoutine = null;
            }

            Time.timeScale = 1;
        }

        public void GameSlow()
        {
            GameSlow(slowTimeScale, slowTime);
        }

        public void GameSlow(float timeScale, float time)
        {
            if (hitRoutine != null)
            {
                StopCoroutine(hitRoutine);
                hitRoutine = null;
                Time.timeScale = 1.0f;
            }
            hitRoutine = StartCoroutine(gameSlow(timeScale, time));

            IEnumerator gameSlow(float timeScale, float time)
            {
                Time.timeScale = timeScale;
                yield return new WaitForSeconds(time * timeScale);
                Time.timeScale = 1.0f;
                hitRoutine = null;
            }
        }

        [Button("TextToCSV")]
        private void TextToCSV()
        {
            string text = "";
            foreach(var v in questDatas)
            {
                text += $"Text_Quest_{v.questCode}_Main,{v.QuestMainName.Replace(",", "\\").Replace("\n", "คั")}\n";
                text += $"Text_Quest_{v.questCode}_Sub,{v.QuestSubName.Replace(",", "\\").Replace("\n", "คั")}\n";
            }
            foreach(var v in scenarioDatas)
            {
                text += $"Text_Scenario_{v.scenarioCode},{v.scenarioText.Replace(",", "\\").Replace("\n", "คั")}\n";
            }
            System.IO.File.WriteAllText("asdf.csv", text, System.Text.Encoding.UTF8);
        }
    }
}