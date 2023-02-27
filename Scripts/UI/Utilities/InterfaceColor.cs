using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Olympus
{
    public class InterfaceColor : InterfaceAnimateObject
    {
        [SerializeField] private Color from;
        [SerializeField] private Color to;
        [SerializeField] private Image[] targetImages;

        [SerializeField] bool R;
        [SerializeField] bool G;
        [SerializeField] bool B;
        [SerializeField] bool A;

        [SerializeField] bool preserveOriginalState;

        protected override void OnAction(float tick, float t, float evaluation)
        {
            for (int i = 0; i < targetImages.Length; i++)
            {
                float r, g, b, a;
                Color origin = targetImages[i].color;
                Color target = targetImages[i].color;

                if(preserveOriginalState == false)
                {
                    target = to;
                }
                r = origin.r;
                g = origin.g;
                b = origin.b;
                a = origin.a;
                
                if (R == true)
                {
                    r = Mathf.Lerp(from.r, target.r, evaluation);
                }
                if (G == true)
                {
                    g = Mathf.Lerp(from.g, target.g, evaluation);
                }
                if (B == true)
                {
                    b = Mathf.Lerp(from.b, target.b, evaluation);
                }
                if (A == true)
                {
                    a = Mathf.Lerp(from.a, target.a, evaluation);
                }

                Color interpolated = new Color(r, g, b, a);
                targetImages[i].color = interpolated;
//                targetImages[i].GraphicUpdateComplete();
                
                targetImages[i].Rebuild(CanvasUpdate.MaxUpdateValue);
            }
        }
#if UNITY_EDITOR
        protected override void Record()
        {
            return;
        }
#endif
    }
}