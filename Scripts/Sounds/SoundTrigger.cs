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
        [Sirenix.OdinInspector.PropertyTooltip("ù �浹 �ÿ��� �Ҹ��� ����ǰ� �� �������� ����")]
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
            // �̹� ����� ���� Collision�� ��� �ش� �ݸ����� ��Ȱ��ȭ�Ѵ�.
            if (isFirstPlayOver)
            {
                // ���� ����� �����µ� ��ȸ�� �ݸ����� ��� �ش� trigger�� �ƿ� Destroy �ұ� �����
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                //gameObject.SetActive(false);
            }
        }

        void TriggerContinue()
        {
            // ����� �־ �߰����� ����� �ʿ��� �� �� �Լ��� ����
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

