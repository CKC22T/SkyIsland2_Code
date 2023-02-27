using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Olympus 
{
    // 얘 추가될 기능에 따라서 안쓸수도 있어여
    public class BGMController : SingletonBase<BGMController>
    {
        private List<string> bgmName;
        private EventInstance currentBGM;
        private Dictionary<string, EventInstance> allBGMSounds;

        void Start()
        {
            allBGMSounds = new Dictionary<string, EventInstance>();
            bgmName = SoundManager.Instance.GetAllSoundsNameBySoundType(SoundType.BGM);

            foreach (var soundName in bgmName)
            {
                SoundManager.Instance.GetSoundByFileName(soundName, out EventInstance eventInstance);
                allBGMSounds.Add(soundName, eventInstance);
            }
        }

        // Update is called once per frame
        void Update()
        {
           
        }

        public bool ChangeBGM(string bgmName)
        {
            currentBGM.stop(STOP_MODE.ALLOWFADEOUT);

            if (!allBGMSounds.ContainsKey(bgmName))
            {
                return false;
            }

            currentBGM = allBGMSounds[bgmName];
            currentBGM.start();

            return true;
        }
    }
}

