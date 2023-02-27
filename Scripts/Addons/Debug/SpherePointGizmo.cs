using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    [ExecuteInEditMode]
    public class SpherePointGizmo : MonoBehaviour
    {
#if UNITY_EDITOR
        public Color gizmoFillColor = new Color(1, 1, 1, 0.3f);
        public Color gizmoLineColor = new Color(1, 1, 1, 0.6f);
        public float radius = 0.5f;

        private SphereCollider sphereCollider;

        // Start is called before the first frame update
        void Start()
        {
            sphereCollider = GetComponent<SphereCollider>();
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = gizmoFillColor;

            if(sphereCollider == null)
            {
                Gizmos.DrawSphere(transform.position, radius);
                Gizmos.color = gizmoLineColor;
                Gizmos.DrawWireSphere(transform.position, radius);
            }
            else
            {
                Gizmos.DrawSphere(transform.position, sphereCollider.radius);
                Gizmos.color = gizmoLineColor;
                Gizmos.DrawWireSphere(transform.position, sphereCollider.radius);
            }

            Gizmos.DrawLine(transform.position, transform.position + transform.forward * radius * 3);
        }
#endif
    }
}