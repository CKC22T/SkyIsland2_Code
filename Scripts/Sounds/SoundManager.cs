using FMOD;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif
// ��� ���� ���ҽ��� �����ϰ� �����ϴ� Ŭ����
namespace Olympus
{
    public enum SoundType
    {
        AMB = 0,
        BGM = 1,
        SFX = 2,
        All
    }

    public class SoundManager : SingletonBase<SoundManager>
    {
        ////////////////////////////////////////////////////// Resource //////////////////////////////////////////////////////

        [FMODUnity.BankRef]
        public List<string> bankpaths;

        private FMOD.Studio.Bank[] banks;
        private List<FMOD.Studio.EventDescription[]> eventDescriptions = new();

        // ��ü ���� �з�
        private Dictionary<string, FMOD.Studio.EventInstance> allSoundsByName = new();
        private Dictionary<SoundType, List<FMOD.Studio.EventInstance>> allSoundsByType = new();

        // ���� ������ Sound Instance
        private Dictionary<SoundType, List<FMOD.Studio.EventInstance>> allInstancesByType = new();


        ////////////////////////////////////////////////////// volume //////////////////////////////////////////////////////
        private Dictionary<SoundType, float> allSoundsVolume = new();
        public Dictionary<SoundType, float> AllSoundsVolume { get { return allSoundsVolume; } }

        protected override void Awake()
        {
            base.Awake();

            allSoundsVolume.Add(SoundType.All, 0.5f);
        }

        void Start()
        {
            SoundType[] soundTypes = (SoundType[])System.Enum.GetValues(typeof(SoundType));
            banks = new FMOD.Studio.Bank[soundTypes.Length - 1];

            // Load Bank Files
            FMODUnity.RuntimeManager.StudioSystem.getBankList(out banks);

            foreach (var bank in banks)
            {
                bank.getPath(out string bankpath);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(bankpath);

                // �� ���Ǵ� BankFile�� EventList ����
                foreach (var item in soundTypes)
                {
                    if (fileName == item.ToString())
                    {
                        bank.getEventList(out FMOD.Studio.EventDescription[] eventDescription);
                        eventDescriptions.Add(eventDescription);

                        List<FMOD.Studio.EventInstance> eventInstanceList = new List<FMOD.Studio.EventInstance>();
                        allInstancesByType.Add(item, eventInstanceList);
                    }
                }
            }

            for (int i = 0; i < eventDescriptions.Count; ++i)
            {
                List<FMOD.Studio.EventInstance> eventInstanceList = new List<FMOD.Studio.EventInstance>();

                foreach (var item in eventDescriptions[i])
                {
                    FMOD.Studio.EventInstance eventInstance;
                    item.createInstance(out eventInstance);

                    // 3D Sound Setting
                    // �� �����Ϸ� ������ �Ѿ�� ���ҽ��� ���� 2D�� ����� ���� . . .��
                    //eventInstance.getChannelGroup(out ChannelGroup channelGroup);
                    //channelGroup.set3DMinMaxDistance(1.0f, 5.0f);
                    

                    item.getPath(out string eventPath);
                    string eventName = System.IO.Path.GetFileName(eventPath);

                    eventInstanceList.Add(eventInstance);
                    allSoundsByName.Add(eventName, eventInstance);
                }

                allSoundsByType.Add(soundTypes[i], eventInstanceList);
                allSoundsVolume.Add(soundTypes[i], 0.5f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Instancing�� Sound���� �����ٸ� �޸𸮸� �ڵ����� ��������
            foreach (var instance in allInstancesByType?.Values)
            {
                if (instance.Count > 0)
                {
                    var toRemove = new HashSet<FMOD.Studio.EventInstance>();

                    foreach (var item in instance)
                    {
                        item.getChannelGroup(out FMOD.ChannelGroup channel);
                        channel.isPlaying(out bool isPlaying);

                        if (!isPlaying && item.isValid())
                        {
                            toRemove.Add(item);
                            item.release();
                            item.clearHandle();
                        }

                    }

                    instance.RemoveAll(toRemove.Contains);
                }
            }

            // Sound Volume�� �׻� Update��
            foreach (var soundDictionary in allSoundsByType)
            {
                List<FMOD.Studio.EventInstance> soundList = soundDictionary.Value;

                foreach (var item in soundList)
                {
                    item.setVolume(allSoundsVolume[SoundType.All] * allSoundsVolume[soundDictionary.Key]);
                }
            }

            foreach (var soundDictionary in allInstancesByType)
            {
                List<FMOD.Studio.EventInstance> soundList = soundDictionary.Value;

                foreach (var item in soundList)
                {
                    item.setVolume(allSoundsVolume[SoundType.All] * allSoundsVolume[soundDictionary.Key]);
                }
            }
        }

        #region Play

        public bool PlayInstance(string fileName, bool playForce = true)
        {
            if (!GetSoundByFileName(fileName, out EventInstance eventInstance))
            {
                LogUtil.LogError($"SoundManager::PlayerInstance Not Found FileName [{fileName}]");
                return false;
            }

            eventInstance.getDescription(out EventDescription eventDescription);
            eventDescription.createInstance(out EventInstance instance);

            SoundType key = allSoundsByType.FirstOrDefault(x => x.Value.Contains(eventInstance)).Key;

            // ���� �ѹ��� �ν��Ͻ� �� ���� ���� ���
            if (!allInstancesByType.ContainsKey(key))
            {
                List<EventInstance> instanceList = new List<EventInstance>();
                allInstancesByType.Add(key, instanceList);
            }

            allInstancesByType[key].Add(instance);

            instance.getChannelGroup(out FMOD.ChannelGroup channel);
            //channel.isPlaying(out bool isPlaying);
            instance.setVolume(allSoundsVolume[key]);
            instance.start();

            return true;
        }

        // �̰� ���� �׽�Ʈ ���..
        private bool PlayInstanceRandom(int soundCount, params string[] fileName)
        {
            List<EventInstance> allSounds = new();

            for (int i = 0; i < soundCount; ++i)
            {
                if (GetSoundByFileName(fileName[i], out EventInstance eventInstance))
                {
                    allSounds.Add(eventInstance);
                }
            }

            // ���⼭ ������ sound�� Instance�� �����Ѵ�.
            int index = Random.Range(0, allSounds.Count);

            allSounds[index].getDescription(out EventDescription eventDescription);
            eventDescription.createInstance(out EventInstance instance);

            SoundType key = allSoundsByType.FirstOrDefault(x => x.Value.Contains(allSounds[index])).Key;

            // ���� �ѹ��� �ν��Ͻ� �� ���� ���� ���
            if (!allInstancesByType.ContainsKey(key))
            {
                List<EventInstance> instanceList = new List<EventInstance>();
                allInstancesByType.Add(key, instanceList);
            }

            allInstancesByType[key].Add(instance);
            instance.start();

            return true;
        }

        public bool PlaySound(string fileName, bool playForce = true)
        {
            if (!GetSoundByFileName(fileName, out EventInstance eventInstance))
            {
                LogUtil.LogError($"SoundManager::PlayerInstance Not Found FileName [{fileName}]");
                return false;
            }

            
            eventInstance.getChannelGroup(out FMOD.ChannelGroup channel);
            channel.isPlaying(out bool isPlaying);

            if (playForce)
            {
                eventInstance.start();
            }

            else
            {
                if (!isPlaying)
                {
                    eventInstance.start();
                }
            }


            return true;
        }

        // ���� ������ ���带 Ȯ���� �����ϰ� ������ִ� ��� (���� �׽�Ʈ ��� ����;)
        public bool PlaySoundRandom(int soundCount ,params string[] fileName)
        {
            List<EventInstance> allSounds = new();
            for(int i = 0; i < soundCount; ++i)
            {
                if (GetSoundByFileName(fileName[i], out EventInstance eventInstance))
                {
                    allSounds.Add(eventInstance);
                }
            }

            int index = Random.Range(0, allSounds.Count);
            EventInstance currentSound = allSounds[index];
            currentSound.getChannelGroup(out FMOD.ChannelGroup channel);

            // ���� ������̴��� ������ ������ �ٽ� �ٽ� ����Ѵ�.
            currentSound.start();
         
            return true;
        }

        // ��� ���̴� ���带 ��� �ߴ�
        public bool PauseSound(string fileName)
        {
            if (!GetSoundByFileName(fileName, out EventInstance eventInstance))
            {
                return false;
            }


            eventInstance.getChannelGroup(out FMOD.ChannelGroup channel);
            channel.isPlaying(out bool isPlaying);

            if (isPlaying)
            {
                eventInstance.setPaused(true);
            }

            return true;
        }

        // �ߴ��� ���带 �簳
        public bool ContinueSound(string fileName)
        {
            if (!GetSoundByFileName(fileName, out EventInstance eventInstance))
            {
                return false;
            }


            eventInstance.getChannelGroup(out FMOD.ChannelGroup channel);
            // ��� ���̶�� bool���� �ߴ��� Ȯ���� ��.
            eventInstance.getPaused(out bool isPaused);

            if (isPaused)
            {
                eventInstance.setPaused(false);
            }

            return true;
        }


        // ���带 ������ ����
        public bool StopSound(string fileName)
        {
            if (!GetSoundByFileName(fileName, out EventInstance eventInstance))
            {
                return false;
            }


            eventInstance.getChannelGroup(out FMOD.ChannelGroup channel);
            channel.isPlaying(out bool isPlaying);

            if (isPlaying)
            {
                eventInstance.stop(STOP_MODE.IMMEDIATE);
            }

            return true;
        }

        public bool StopSound(string fileName, STOP_MODE stopMode)
        {
            if (!GetSoundByFileName(fileName, out EventInstance eventInstance))
            {
                return false;
            }


            eventInstance.getChannelGroup(out FMOD.ChannelGroup channel);
            channel.isPlaying(out bool isPlaying);

            if (isPlaying)
            {
                eventInstance.stop(stopMode);
            }

            return true;
        }

        #endregion

        // ���� ���ҽ� 
        #region GetSoundResources
        public List<string> GetAllSoundsNameBySoundType(SoundType soundType)
        {
            if (!allSoundsByType.ContainsKey(soundType))
            {
                return null;
            }

            List<string> soundNameByType = new List<string>();

            foreach (var eventInstance in allSoundsByType[soundType])
            {
                eventInstance.getDescription(out FMOD.Studio.EventDescription eventDescription);
                eventDescription.getPath(out string eventPath);
                string soundName = System.IO.Path.GetFileName(eventPath);
                soundNameByType.Add(soundName);
            }

            //return GetAllSoundsNameBySoundType(soundType).ToArray<string>();
            return soundNameByType;
        }

        public bool GetSoundByFileName(string fileName, out FMOD.Studio.EventInstance eventInstance)
        {
            if (allSoundsByName.TryGetValue(fileName, out eventInstance))
            {
                return true;
            }

            return false;
        }

        public bool GetSoundsByType(SoundType soundType, out FMOD.Studio.EventInstance[] eventInstance)
        {

            if (allSoundsByType.TryGetValue(soundType, out List<EventInstance> soundList))
            {
                eventInstance = soundList.ToArray<EventInstance>();
                return true;
            }

            eventInstance = null;
            return false;
        }
        #endregion

        // ���� �з��� ���� ��ü ������ ����
        #region SetVolume
        public void ControlVolumeBySoundType(float volume, SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.All:
                    SetVolumeForAllSounds(volume);
                    break;

                // AMB�� SFX�� ��� �Ҹ��� ������
                case SoundType.SFX:
                    SetVolumeBySoundType(SoundType.SFX, volume);
                    SetVolumeBySoundType(SoundType.AMB, volume);
                    break;
                default:
                    SetVolumeBySoundType(soundType, volume);
                    break;
            }
        }

        private void SetVolumeBySoundType(SoundType soundType, float volume)
        {
            allSoundsVolume[soundType] = volume;

            foreach (var item in allSoundsByType[soundType])
            {
                item.setVolume(volume);
            }

            if (allInstancesByType?[soundType].Count > 0)
            {
                foreach (var item in allInstancesByType?[soundType])
                {
                    item.setVolume(volume);
                }
            }
        }

        private void SetVolumeForAllSounds(float volume)
        {
            allSoundsVolume[SoundType.All] = volume;

            // ��ü ������ ������ ���δ�.
            foreach (var soundPair in allSoundsByType)
            {
                foreach (var item in soundPair.Value)
                {
                    float localVolume = volume * allSoundsVolume[soundPair.Key];
                    item.setVolume(localVolume);
                }
            }

            foreach (var instance in allInstancesByType)
            {
                if (instance.Value.Count > 0)
                {
                    foreach (var item in instance.Value)
                    {
                        float localVolume = volume * allSoundsVolume[instance.Key];
                        item.setVolume(localVolume);
                    }
                }
            }
        }

        #endregion
    }

}