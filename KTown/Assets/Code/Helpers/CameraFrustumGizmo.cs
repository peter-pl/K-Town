using UnityEngine;

namespace Misc
{
    public class CameraFrustumGizmo : MonoBehaviour
    {
        public bool IsTurnedOn;
        public Color GizmoColor = Color.cyan;
        public float FarClipPlane = 2;
        public virtual void OnDrawGizmos()
        {
            if (IsTurnedOn)
            {
                Camera camera = FindObjectOfType<Camera>();
                float FOV = camera.fieldOfView;

                Matrix4x4 temp = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                
                Gizmos.color = GizmoColor;
                if (camera.orthographic)
                {
                    float spread = FarClipPlane - camera.nearClipPlane;
                    float center = (FarClipPlane + camera.nearClipPlane) * 0.5f;
                    Gizmos.DrawWireCube(new Vector3(0, 0, center), new Vector3(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2, spread));
                }
                else
                {
                    Gizmos.DrawFrustum(new Vector3(0, 0, (camera.nearClipPlane)), FOV, FarClipPlane, camera.nearClipPlane, camera.aspect);
                }
                Gizmos.matrix = temp;
            }
        }
    }
}