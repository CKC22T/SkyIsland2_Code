using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Olympus
{
    public class CameraSequenceVolumeEnterLevelEvent : LevelEventBase
    {
        [SerializeField] CameraStateData data;
        public override void OnLevelEvent(EntityBase entity)
        {
            PlayerCamera camera = PlayerCamera.Instance;

            camera.SetState(data, 0);
        }
    }
}