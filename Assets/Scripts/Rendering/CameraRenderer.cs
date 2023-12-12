using UnityEngine;

namespace RecoDeli.Scripts.Rendering
{
    [ExecuteAlways]
    public class CameraRenderer : RenderLayerProvider
    {
        [SerializeField] private Camera assignedCamera;
        protected override void AssignToRenderer()
        {
            if (assignedCamera == null) return;

            if(assignedCamera.targetTexture != layer)
            {
                assignedCamera.targetTexture = layer;
            }

            if (!layer.IsCreated())
            {
                assignedCamera.ResetAspect();
            }
        }

        protected override void ClearRenderer()
        {
            base.ClearRenderer();

            if (assignedCamera == null) return;

            assignedCamera.targetTexture = null;
        }
    }
}
