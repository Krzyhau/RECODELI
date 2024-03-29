using RecoDeli.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RecoDeli.Scripts.Controllers
{
    [ExecuteInEditMode]
    public class CameraProjectionCorrector : MonoBehaviour
    {
        [SerializeField] private Camera controlledCamera;
        [SerializeField] private SimulationInterface simulationInterface;

        private void Update()
        {
            RecalculateCameraProjection();
        }

        private void RecalculateCameraProjection()
        {
            if (simulationInterface == null)
            {
                return;
            }

            var near = controlledCamera.nearClipPlane;
            var far = controlledCamera.farClipPlane;
            var fovScale = Mathf.Tan(Mathf.Deg2Rad * controlledCamera.fieldOfView * 0.5f) * near;

            var editorWidth = simulationInterface.InstructionEditorWidth;
            var screenWidth = controlledCamera.pixelWidth;
            var offset = (editorWidth / screenWidth);

            var left = (offset - 1.0f) * fovScale * controlledCamera.aspect;
            var right = (offset + 1.0f) * fovScale * controlledCamera.aspect;
            var top = 1.0f * fovScale;
            var bottom = -1.0f * fovScale;

            float x = 2.0F * near / (right - left);
            float y = 2.0F * near / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(far + near) / (far - near);
            float d = -(2.0F * far * near) / (far - near);
            float e = -1.0F;

            controlledCamera.projectionMatrix = new Matrix4x4(
                new(x, 0, 0, 0),
                new(0, y, 0, 0),
                new(a, b, c, e),
                new(0, 0, d, 0)
            );
        }
    }
}