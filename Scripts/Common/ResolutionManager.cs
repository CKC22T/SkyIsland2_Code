using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Olympus
{
    public class ResolutionManager : SingletonBase<ResolutionManager>
    {
        int width = 1920;
        int height = 1080;

        public bool fullScreenMode = true;

        // Start is called before the first frame update
        void Start()
        {
            //SetResolution();
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Backslash))
            //{
            //    if(fullScreenMode)
            //    {
            //        SetFullScreenMode(false);
            //    }

            //    else
            //    {
            //        SetFullScreenMode(true);
            //    }
            //}
        }

        public void Initialize()
        {
            SetFullScreenMode(true);
        }

        private void SetFreeResolution()
        {
            if (!Camera.main)
            {
                return;
            }

            int deviceWidth = Screen.width; // 기기 너비 저장
            int deviceHeight = Screen.height; // 기기 높이 저장

            width = deviceWidth;
            height = (deviceWidth * 9) / 16;

            Screen.SetResolution(width, height, UnityEngine.FullScreenMode.FullScreenWindow);

        }

        public void SetResolution(int width, int height)
        {
            Screen.SetResolution(width, height, Screen.fullScreenMode);
        }
        
        public void SetFullScreenMode(bool isFullScreenMode)
        {
            fullScreenMode = isFullScreenMode;

            if (isFullScreenMode)
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }

            else
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
        }
    }
}

