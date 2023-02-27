using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class OlympusFmodSound : MonoBehaviour
    {
        [Sirenix.OdinInspector.PropertyTooltip("ù �浹 �ÿ��� �Ҹ��� ����ǰ� �� �������� ����")]
        public bool soundPlay = false;

        public string soundName;
        private FMODUnity.EventReference soundPath;
        private FMOD.Studio.EventInstance eventInstance;
        
        void Start()
        {
            //string fileName = System.IO.Path.GetFileNameWithoutExtension(soundPath.Path);
            //SoundManager.Instance.GetSoundByFileName(soundName, out eventInstance);
        }

        void Update()
        {
            if (!soundPlay)
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }

            else
            {
                eventInstance.getChannelGroup(out FMOD.ChannelGroup channel);
                channel.isPlaying(out bool isPlaying);

                if (!isPlaying)
                {
                    eventInstance.start();
                }
            }
        }
    }
}