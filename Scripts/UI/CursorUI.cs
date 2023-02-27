using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class CursorUI : UIBase
    {
        public GameObject cursorObject;

        // Update is called once per frame
        void Update()
        {
            cursorObject.transform.position = Input.mousePosition;
            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f);
            if (PlayerController.Instance.PlayerEntity)
            {
                Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(PlayerController.Instance.PlayerEntity.CenterPosition.position);
                dir.z = dir.y;
                dir.y = 0.0f;

                dir.Normalize();
                rotation = Quaternion.Euler(0.0f, 0.0f, -Quaternion.LookRotation(dir).eulerAngles.y);
            }
            cursorObject.transform.rotation = rotation;
        }
    }
}