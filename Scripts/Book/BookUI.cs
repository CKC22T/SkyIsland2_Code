using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Olympus
{
    public class BookUI : MonoBehaviour
    {
        public static BookUI Instance { get; private set; }
        #region Inspector
        [TabGroup("Component"), SerializeField] private TextMeshProUGUI text;
        #endregion
        #region Value
        private int mIndex = 0;
        #endregion

        #region Event
        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }
        #endregion
        #region Function
        public void Open(int _index)
        {
            if (TextTable.Instance.Current != SystemLanguage.Korean)
            {
                gameObject.SetActive(true);
                text.text = "";
                mIndex = _index;
                StopAllCoroutines();

                StartCoroutine(TextCor());
            }
        }
        public void Close()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator TextCor()
        {
            yield return new WaitForSeconds(1.0f);
            text.text = TextTable.Instance.Get($"Text_Story_{mIndex}");
        }
        #endregion
    }
}