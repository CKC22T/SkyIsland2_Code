using FMOD;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Olympus
{

    public class SettingUI : UIBase
    {
        [field: SerializeField] public TMP_Text  textResolution;
        [field: SerializeField] public TMP_Text textScreenMode;
        [field: SerializeField] public TMP_Text textPresset;
        [field: SerializeField] public TMP_Text textVSync;

        [field: SerializeField] private bool verticalSyncFlag = false;
        [field: SerializeField] private int graphicPresetIndex = 2;

        // Volume
        [field: SerializeField] public SerializableDictionary<SoundType, UnityEngine.UI.Slider> volumeSliders = new();

        // Resolution
        [field: SerializeField] public List<string> resolutionList = new();
        private int resolutionIndex = 1;

        // Screen
        private bool isFullScreenMode = true;

        private void Start()
        {
            TextTable.Instance.AddEvent(this, () =>
            {
                textScreenMode.text = TextTable.Instance.Get(isFullScreenMode ? "Text_Setting_WinFull" : "Text_Setting_WinWin");

                switch (QualitySettings.GetQualityLevel())
                {
                    case 0:
                        textPresset.text = TextTable.Instance.Get("Text_Setting_GraphicLow");
                        break;
                    case 1:
                        textPresset.text = TextTable.Instance.Get("Text_Setting_GraphicMedium");
                        break;
                    case 2:
                        textPresset.text = TextTable.Instance.Get("Text_Setting_GraphicHigh");
                        break;
                }

                textVSync.text = verticalSyncFlag ? TextTable.Instance.Get("Text_Setting_On") : TextTable.Instance.Get("Text_Setting_Off");
            });


        }

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }

        public void Hide()
        {
            UIManager.Hide(Id);
        }

        #region Setvolume
        public void ControlVolume(int soundType)
        {

            //이때 전체 볼륨을 조정할 경우 하위 볼륨들도 같이 조정해준다.
            switch (soundType)
            {
                case (int)SoundType.All:
                    SetAllVolume();
                    break;

                default:
                    // 전체 볼륨에 전부 종속되게 제작
                    float allVolume = volumeSliders[SoundType.All].value;

                    if (Mathf.Approximately(allVolume, 0.0f))
                    {
                        SetVolume(SoundType.BGM, allVolume);
                        SetVolume(SoundType.SFX, allVolume);
                    }

                    else
                    {
                        SetVolume((SoundType)soundType);
                    }

                    break;
            }
        }


        private void SetAllVolume()
        {
            // 전체 볼륨을 조정할 시 하위 볼륨도 강제로 종속되게 함.

            UnityEngine.UI.Slider allSlider = volumeSliders[SoundType.All];
            SoundManager.Instance.ControlVolumeBySoundType(allSlider.value * 0.01f, SoundType.All);

            SetVolume(SoundType.All);
         //   SetVolume(SoundType.BGM, allSlider.value);
         //   SetVolume(SoundType.SFX, allSlider.value);
        }

        private void SetVolume(SoundType soundType)
        {
            UnityEngine.UI.Slider slider = volumeSliders[soundType];
            SetVolume(soundType, slider.value);
        }

        private void SetVolume(SoundType soundType, float volume)
        {
            UnityEngine.UI.Slider slider = volumeSliders[soundType];
            slider.value = volume;

            SoundManager.Instance.ControlVolumeBySoundType(volume * 0.01f, soundType);
            TextMeshProUGUI text = slider.GetComponentInChildren<TextMeshProUGUI>();
            text.text = volume.ToString();
        }
        #endregion

        #region SetResolution
        public void ChangeResolution(bool isButtonR)
        {
            if (isButtonR)
            {
                if (resolutionIndex == resolutionList.Count - 1)
                {
                    return;
                }

                ++resolutionIndex;
            }

            else
            {
                if (resolutionIndex == 0) { return;}

                --resolutionIndex;
            }

            textResolution.text = resolutionList[resolutionIndex];

            switch (resolutionIndex)
            {
                case 0:
                    ResolutionManager.Instance.SetResolution(1280, 720);
                    break;
                case 1:
                    ResolutionManager.Instance.SetResolution(1920, 1080);
                    break;
                case 2:
                    ResolutionManager.Instance.SetResolution(2560, 1440);
                    break;
                case 3:
                    ResolutionManager.Instance.SetResolution(3840, 2160);
                    break;
                default:
                    break;
            }

        }

        public void SetGraphicsPreset(bool isButtonR)
        {

            if(isButtonR == true)
            {
                if(QualitySettings.GetQualityLevel() < 3)
                {
                    QualitySettings.IncreaseLevel();
                }
            }
            else
            {
                if (QualitySettings.GetQualityLevel() > 0)
                {
                    QualitySettings.DecreaseLevel();
                }
            }

            switch(QualitySettings.GetQualityLevel())
            {
                case 0:
                    textPresset.text = TextTable.Instance.Get("Text_Setting_GraphicLow");
                    break;
                case 1:
                    textPresset.text = TextTable.Instance.Get("Text_Setting_GraphicMedium");
                    break;
                case 2:
                    textPresset.text = TextTable.Instance.Get("Text_Setting_GraphicHigh");
                    break;
            }
        }

        public void SwitchVerticalSynchronization()
        {
            verticalSyncFlag = !verticalSyncFlag;
            QualitySettings.vSyncCount = verticalSyncFlag ? 1 : 0;
            textVSync.text = verticalSyncFlag ? TextTable.Instance.Get("Text_Setting_On") : TextTable.Instance.Get("Text_Setting_Off");
        }

        public void SetFullScreenMode()
        {
            isFullScreenMode = !isFullScreenMode;
            ResolutionManager.Instance.SetFullScreenMode(isFullScreenMode);
        }
        #endregion

        #region SetLanguage
        public void ChangeLanguageLeft()
        {
            int index = TextTable.Instance.CurrentIndex - 1;
            if (index < 0)
                index = TextTable.Instance.Support.Length - 1;
            TextTable.Instance.CurrentIndex = index;
        }
        public void ChangeLanguageRight()
        {
            int index = (TextTable.Instance.CurrentIndex + 1) % TextTable.Instance.Support.Length;
            TextTable.Instance.CurrentIndex = index;
        }
        #endregion
    }
}