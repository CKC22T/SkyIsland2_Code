using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    [ExecuteInEditMode]
    public class BoxGizmo : MonoBehaviour
    {
#if UNITY_EDITOR
        public Color gizmoFillColor = new Color(1, 1, 1, 0.3f);
        public Color gizmoLineColor = new Color(1, 1, 1, 0.6f);

        private BoxCollider boxCollider;

        // Start is called before the first frame update
        void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
        }


        private void OnDrawGizmos()
        {
            if(boxCollider == null)
            {
                return;
            }

            Vector3 currentPosition = boxCollider.center - transform.position;
            Gizmos.matrix = Matrix4x4.TRS(this.transform.TransformPoint(transform.position), transform.rotation, transform.lossyScale);
            Gizmos.color = gizmoFillColor;

            Gizmos.DrawCube(currentPosition, boxCollider.size);
            Gizmos.color = gizmoLineColor;
            Gizmos.DrawWireCube(currentPosition, boxCollider.size);
        }
#endif
    }
}