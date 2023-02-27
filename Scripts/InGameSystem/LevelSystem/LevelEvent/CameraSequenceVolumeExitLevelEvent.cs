using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class CameraSequenceVolumeExitLevelEvent : LevelEventBase
    {
        public override void OnLevelEvent(EntityBase entity)
        {
            PlayerCamera camera = PlayerCamera.Instance;

            camera.ReleaseState();
        }
    }
}