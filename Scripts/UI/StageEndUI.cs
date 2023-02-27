using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Olympus
{
    public class StageEndUI : UIBase
    {
        public TextMeshProUGUI islandName;

        //public Image nextImage;
        public IslandType currentIslandType;
        public SceneType nextSceneType;

        public float nextTime = 1.5f;
        public float timer = 0.0f;

        public GameObject selectAbilityUI;
        public GameObject scoreBoardUI;

        public AnimationCurve scoreBoardAniCurve;
        public TextMeshProUGUI stageScoreText;
        public TextMeshProUGUI achievementText;
        public Image achievementImage;

        public TextMeshProUGUI clearTimeText;
        public TextMeshProUGUI killCountText;
        public TextMeshProUGUI remainHealthText;
        //public TextMeshProUGUI deathCountText;

        public Image swordSkillImage;
        public Image spearSkillImage;
        public Image hammerSkillImage;

        public Animator logBoardAnimator;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            PlayerController.Instance.InputLock(LockType.FromGUI);
            timer = 0.0f;
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        public void InitUI(IslandType islandType, SceneType nextSceneType)
        {
            this.nextSceneType = nextSceneType;
            currentIslandType = islandType;
            islandName.text = TextTable.Instance.Get(IslandNameInfo.ISLAND_NAME[islandType]);
            //GameDataManager.Instance.SetScore();
            ChangeScoreBoardUI();
        }

        public void Update()
        {
            //if(!nextImage.isActiveAndEnabled)
            //{
            //    return;
            //}

            //if(Input.GetKey(KeyCode.F))
            //{
            //    timer += Time.deltaTime;
            //}
            //else if(timer > 0.0f)
            //{
            //    timer -= Time.deltaTime;
            //}
            //else
            //{
            //    timer = 0.0f;
            //}

            if(Input.GetKeyDown(KeyCode.F))
            {
                Main.Instance.ChangeScene(nextSceneType);
                UIManager.Hide(UIList.StageEndUI);
            }

            //nextImage.fillAmount = timer / nextTime;
            //if(timer > nextTime)
            //{
            //    Main.Instance.ChangeScene(nextSceneType);
            //    UIManager.Hide(UIList.StageEndUI);
            //}
        }

        public void SelectAbilityHP()
        {
            GameDataManager.Instance.healthLevel++;
            ChangeScoreBoardUI();
        }

        public void SelectAbilityAttackSpeed()
        {
            GameDataManager.Instance.atkSpeedLevel++;
            ChangeScoreBoardUI();
        }

        public void SelectAbilityCritical()
        {
            GameDataManager.Instance.criticalLevel++;
            ChangeScoreBoardUI();
        }

        private void ChangeScoreBoardUI()
        {
            //selectAbilityUI.SetActive(false);
            scoreBoardUI.SetActive(true);
            StartCoroutine(ShowScoreBoard());
            SetAdventureLog();
        }

        private IEnumerator ShowScoreBoard()
        {
            swordSkillImage.gameObject.SetActive(GameDataManager.Instance.weaponLevelSword == 0);
            spearSkillImage.gameObject.SetActive(GameDataManager.Instance.weaponLevelSpear == 0);
            hammerSkillImage.gameObject.SetActive(GameDataManager.Instance.weaponLevelHammer == 0);

            float timer = 0.0f;

            float achievment = GameDataManager.Instance.presentScore / (float)GameData.OLYMPUS_ACHIEVEMNET[currentIslandType];
            
            while(timer < 1.0f)
            {
                timer += Time.deltaTime;

                float t = scoreBoardAniCurve.Evaluate(timer);
                stageScoreText.text = string.Format("{0:#,###0}", Mathf.FloorToInt(Mathf.Lerp(0.0f, GameDataManager.Instance.presentScore, t)));
                achievementText.text = $"{Mathf.FloorToInt(Mathf.Lerp(0.0f, achievment * 100.0f, t))} %";
                achievementImage.fillAmount = Mathf.Lerp(0.0f, achievment, t);

                yield return null;
            }

            stageScoreText.text = Mathf.FloorToInt(GameDataManager.Instance.presentScore).ToString();
            achievementText.text = $"{achievment * 100.0f} %";
            achievementImage.fillAmount = achievment;

            yield return null;
        }

        private void SetAdventureLog()
        {
            int clearTime = Mathf.FloorToInt(GameDataManager.Instance.presentTime);
            int clearTimeM = clearTime / 60;
            int cleatTimeS = clearTime % 60;

            int killCount = 0;

            foreach(var count in GameDataManager.Instance.enemyKillCounts.Values)
            {
                killCount += count;
            }

            clearTimeText.text = $"{clearTimeM}:{cleatTimeS}";
            killCountText.text = $"{killCount}";
            remainHealthText.text = $"{GameDataManager.Instance.remainHealth * 100 / PlayerController.Instance.PlayerEntity.EntityData.maxHealth}%";
            //deathCountText.text = $"{GameDataManager.Instance.deathCount}";
        }

        private void SetSelectAbilityUI()
        {
            selectAbilityUI.SetActive(true);
            scoreBoardUI.SetActive(false);
        }

        public void ShowLogBoard()
        {
            logBoardAnimator.ResetTrigger("Hide");
            logBoardAnimator.SetTrigger("Show");
        }

        public void HideLogBoard()
        {
            logBoardAnimator.ResetTrigger("Show");
            logBoardAnimator.SetTrigger("Hide");
        }
    }
}