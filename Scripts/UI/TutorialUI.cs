using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class TutorialUI : UIBase
    {
        [SerializeField] private TextMeshProUGUI scriptContents;
        public float autoHideTime = 5.0f;
        public float timer = 0.0f;

        private Coroutine talkCoroutine;
        private ScenarioData scenarioBuffer;

        public GameObject KeyPanel;
        public SerializableDictionary<TutorialType, GameObject> panels = new();

        private new void Awake()
        {
            base.Awake();
            ShowScript(null);
        }
        private void Start()
        {
            TextTable.Instance.AddEvent(this, () =>
            {
                if (scenarioBuffer)
                    scriptContents.text = scenarioBuffer.GetText();
            });
        }

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            ShowScript(scenarioBuffer);

            timer = 0.0f;
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        public void ShowScript(ScenarioData scenarioData)
        {
            if(talkCoroutine != null)
            {
                StopCoroutine(talkCoroutine);
                talkCoroutine = null;
            }

            if (scenarioData)
            {
                scenarioBuffer = scenarioData;
                talkCoroutine = StartCoroutine(TalkCor(scenarioData.GetText()));

                KeyPanel.SetActive(false);
                if (TutorialInfo.TUTORIAL_TYPE.ContainsKey(scenarioData.scenarioCode))
                {
                    KeyPanel.SetActive(true);
                    foreach(var kv in panels)
                    {
                        kv.Value.SetActive(kv.Key == TutorialInfo.TUTORIAL_TYPE[scenarioData.scenarioCode]);
                    }
                }
            }
            else
            {
                scriptContents.text = "";
            }
        }

        private IEnumerator TalkCor(string _text)
        {
            float timer = 0;
            while (timer < _text.Length * 0.05f)
            {
                scriptContents.text = _text.Substring(0, (int)((_text.Length - 1) * timer / (_text.Length * 0.05f)));

                timer += Time.deltaTime;

                yield return null;
            }
            scriptContents.text = _text;
        }
    }
}