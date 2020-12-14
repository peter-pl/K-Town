using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Misc
{
    public class GizmoTransform : MonoBehaviour
    {
        public bool IsOverriding = false;
        public Color ColorOverride = Color.gray;
        public float LinesLenght = 1f;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            if (IsOverriding)
                Gizmos.color = ColorOverride;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * LinesLenght);
            Gizmos.color = Color.red;
            if (IsOverriding)
                Gizmos.color = ColorOverride;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * LinesLenght);
            Gizmos.color = Color.green;
            if (IsOverriding)
                Gizmos.color = ColorOverride;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * LinesLenght);
        }
    }
}