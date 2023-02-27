using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public enum SoundTriggerType
    {
        Once,
        Continue
    }

    public class SoundTrigger : MonoBehaviour
    {
        [Sirenix.OdinInspector.PropertyTooltip("첫 충돌 시에만 소리가 재생되게 할 것인지를 결정")]
        public SoundTriggerType soundTriggerType = SoundTriggerType.Once;

        private bool isFirstPlayOver = false;
        private bool isSoundStart = false;

        public FMODUnity.EventReference soundPath;
        private FMOD.Studio.EventInstance eventInstance;

        private FMODUnity.EventHandler eventHandler;


        void Start()
        {
            //string fileName = System.IO.Path.GetFileNameWithoutExtension(soundPath.Path);
            //SoundManager.Instance.GetSoundByFileName(fileName, out eventInstance);
        }

        void Update()
        {
            eventInstance.getChannelGroup(out FMOD.ChannelGroup channel);
            channel.isPlaying(out bool isPlaying);

            if (isSoundStart && !isPlaying)
            {
                EndSound();
            }

            switch (soundTriggerType)
            {
                case SoundTriggerType.Once:
                    TriggerOnce();
                    break;

                case SoundTriggerType.Continue:
                    TriggerContinue();
                    break;

                default:
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            StartSound();

        }

        void OnTriggerExit()
        {

        }

        void TriggerOnce()
        {
            // 이미 재생이 끝난 Collision일 경우 해당 콜리전을 비활성화한다.
            if (isFirstPlayOver)
            {
                // 사운드 재생이 끝났는데 일회용 콜리전일 경우 해당 trigger를 아예 Destroy 할까 고민중
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                //gameObject.SetActive(false);
            }
        }

        void TriggerContinue()
        {
            // 재생에 있어서 추가적인 기능이 필요할 시 이 함수에 제작
        }

        void StartSound()
        {
            isSoundStart = true;
            eventInstance.start();
        }

        void EndSound()
        {
            if (!isFirstPlayOver && (soundTriggerType == SoundTriggerType.Once))
            {
                isFirstPlayOver = true;
            }

            isSoundStart = false;
        }

    }
}

