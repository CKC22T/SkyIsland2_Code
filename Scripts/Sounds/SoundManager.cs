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
// 모든 사운드 리소스를 저장하고 관리하는 클래스
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

        // 전체 사운드 분류
        private Dictionary<string, FMOD.Studio.EventInstance> allSoundsByName = new();
        private Dictionary<SoundType, List<FMOD.Studio.EventInstance>> allSoundsByType = new();

        // 현재 생성된 Sound Instance
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

                // 실 사용되는 BankFile의 EventList 저장
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
                    // 를 적용하려 했으나 넘어온 리소스가 전부 2D인 관계로 생략 . . .ㅠ
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
            // Instancing된 Sound들이 끝났다면 메모리를 자동으로 해제해줌
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

            // Sound Volume을 항상 Update함
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

            // 아직 한번도 인스턴싱 된 적이 없을 경우
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

        // 이것 또한 테스트 기능..
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

            // 여기서 선정된 sound의 Instance를 생성한다.
            int index = Random.Range(0, allSounds.Count);

            allSounds[index].getDescription(out EventDescription eventDescription);
            eventDescription.createInstance(out EventInstance instance);

            SoundType key = allSoundsByType.FirstOrDefault(x => x.Value.Contains(allSounds[index])).Key;

            // 아직 한번도 인스턴싱 된 적이 없을 경우
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

        // 여러 가지의 사운드를 확률로 랜덤하게 재생해주는 기능 (아직 테스트 기능 ㅎㅎ;)
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

            // 현재 재생중이더라도 무조건 강제로 다시 다시 재생한다.
            currentSound.start();
         
            return true;
        }

        // 재생 중이던 사운드를 잠시 중단
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

        // 중단한 사운드를 재개
        public bool ContinueSound(string fileName)
        {
            if (!GetSoundByFileName(fileName, out EventInstance eventInstance))
            {
                return false;
            }


            eventInstance.getChannelGroup(out FMOD.ChannelGroup channel);
            // 재생 중이라고 bool값이 뜨는지 확인할 것.
            eventInstance.getPaused(out bool isPaused);

            if (isPaused)
            {
                eventInstance.setPaused(false);
            }

            return true;
        }


        // 사운드를 완전히 정지
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

        // 사운드 리소스 
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

        // 사운드 분류에 따른 전체 음악을 조정
        #region SetVolume
        public void ControlVolumeBySoundType(float volume, SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.All:
                    SetVolumeForAllSounds(volume);
                    break;

                // AMB와 SFX는 묶어서 소리를 조절함
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

            // 전체 사운드의 볼륨을 줄인다.
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