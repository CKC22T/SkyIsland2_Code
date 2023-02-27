using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Olympus
{
    public class GameEndUI : UIBase
    {
        public TextMeshProUGUI playTimeText;
        public TextMeshProUGUI killEnemyCountText;
        public TextMeshProUGUI openBoxPuriCountText;

        public SerializableDictionary<int, GameObject> boxpuriIcons;
        public float openDelay = 0.1f;

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        public void OnEnable()
        {
            int playTime = Mathf.FloorToInt(GameDataManager.Instance.totalGameTime * 100);
            int killEnemyCount = GameDataManager.Instance.totalKillEnemyCount;
            int openBoxPuriCount = GameDataManager.Instance.TotalOpenBoxPuriCount;

            playTimeText.text = $"{playTime / 6000} : {playTime / 100 % 60} : {playTime % 100}";
            killEnemyCountText.text = killEnemyCount.ToString();
            openBoxPuriCountText.text = openBoxPuriCount.ToString();

            StartCoroutine(ShowOpenBoxPuri());

            IEnumerator ShowOpenBoxPuri()
            {
                foreach (var kv in boxpuriIcons)
                {
                    kv.Value.transform.GetChild(0).gameObject.SetActive(GameDataManager.Instance.openBoxChecksum.Contains(kv.Key));
                    kv.Value.SetActive(true);
                    yield return new WaitForSeconds(openDelay);
                }
            }
        }

        private void OnDisable()
        {
            foreach (var kv in boxpuriIcons)
            {
                //kv.Value.transform.GetChild(0).gameObject.SetActive(false);
                kv.Value.SetActive(false);
            }
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                //Off
                LogUtil.Log("Book Close");
            }
        }
    }
}