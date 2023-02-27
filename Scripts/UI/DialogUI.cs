using FMOD;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class DialogUI : UIBase
    {
        public TextMeshProUGUI scriptText;
        public ScenarioData scenarioData;
        public EntityBase dialogTarget;
        public Transform dialogTransform;
        public GameObject dialogTutorialObject;

        private Coroutine talkCoroutine;
        private Coroutine endCoroutine;
        private string textBuffer = "";

        public override void Show(UnityAction callback = null)
        {
            animationTimer.Reset();
            base.Show(callback);
            scriptText.text = "";
            DisplayQuote(textBuffer);
            //SoundManager.Instance.PlaySound("UI_Speechbubble");
        }

        public override void Hide(UnityAction callback = null)
        {
            if (talkCoroutine != null)
            {
                StopCoroutine(talkCoroutine);
                talkCoroutine = null;
            }
            if(endCoroutine != null)
            {
                StopCoroutine(endCoroutine);
                endCoroutine = null;
            }
            base.Hide(callback);
        }

        void DisplayQuote(string text)
        {
            if(talkCoroutine != null)
            {
                StopCoroutine(talkCoroutine);
                SoundManager.Instance.StopSound("UI_Speechbubble");
                talkCoroutine = null;
            }

            SoundManager.Instance.PlaySound("UI_Speechbubble");
            talkCoroutine = StartCoroutine(TalkCor(text));
        }

        public void SetScenario(ScenarioData scenarioData)
        {
            this.scenarioData = scenarioData;
            textBuffer = scenarioData.GetText();
            dialogTutorialObject.SetActive(scenarioData.scenarioCode <= 10108);

            if (!scenarioData.possiblePlayerControl)
            {
                if (!PlayerController.Instance.IsControlLocked)
                {
                    PlayerController.Instance.PlayerEntity.SetActionType(ActionType.Idle);
                }

                PlayerController.Instance.InputLock(LockType.FromGUI);
            }

            DisplayQuote(textBuffer);
        }

        public void SetDialogTarget(EntityBase entity)
        {
            dialogTarget = entity;
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                EndDailog();
            }

            if(dialogTarget)
            {
                dialogTransform.position = Camera.main.WorldToScreenPoint(dialogTarget.transform.position + dialogTarget.Offset);
            }
        }

        public void EndDailog()
        {

            if (talkCoroutine != null)
            {
                StopCoroutine(talkCoroutine);
                talkCoroutine = null;
                scriptText.text = textBuffer;
                endCoroutine = StartCoroutine(EndCor());
                return;
            }
            if(endCoroutine != null)
            {
                StopCoroutine(endCoroutine);
                endCoroutine = null;
            }

            UIManager.Hide(UIList.DialogUI);
            if (!scenarioData.possiblePlayerControl)
            {
                PlayerController.Instance.InputUnLock(LockType.FromGUI);
            }

            GameManager.Instance.EndScenario(scenarioData.scenarioCode);
        }

        private IEnumerator TalkCor(string _text)
        {

            float timer = 0;
            while (timer < _text.Length * 0.025f)
            {
                scriptText.text = _text.Substring(0, (int)((_text.Length - 1) * timer / (_text.Length * 0.025f)));
                timer += Time.deltaTime;
                yield return null;
            }
            scriptText.text = _text;

            //float delay = 1 + _text.Length * 0.05f;

            //yield return new WaitForSeconds(delay);
            talkCoroutine = null;

            if(scenarioData?.possiblePlayerControl == true)
            {
                endCoroutine = StartCoroutine(EndCor());
            }
            //Close();
        }

        private IEnumerator EndCor()
        {
            yield return new WaitForSeconds(1 + scriptText.text.Length * 0.05f);
            endCoroutine = null;
            EndDailog();
        }
    }
}