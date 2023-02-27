using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ScriptTrigger : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<int, LevelEventBase> scriptLevelEvent;

        // Start is called before the first frame update
        void Start()
        {
            GameManager.Instance.ConnectScriptTrigger(this);
        }

        private void OnDestroy()
        {
            GameManager.Instance.DisconnectScriptTrigger();
        }

        public void OnTrigger(int code)
        {
            if(scriptLevelEvent.ContainsKey(code))
            {
                scriptLevelEvent[code].OnLevelEvent(null);
            }
        }
    }
}