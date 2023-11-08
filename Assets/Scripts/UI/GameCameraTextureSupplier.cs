using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    [ExecuteAlways]
    public class GameCameraTextureSupplier : MonoBehaviour
    {
        [SerializeField] private Camera cameraToRender;
        [SerializeField] private RawImage imageTextureDisplayer;

        private RenderTexture suppliedRenderTexture;

        public void Update()
        {
            if (cameraToRender == null || imageTextureDisplayer == null)
            {
                return;
            }

            if (suppliedRenderTexture == null)
            {
                suppliedRenderTexture = new RenderTexture(Screen.width, Screen.height, 1);
                suppliedRenderTexture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat;
            }
            if (imageTextureDisplayer.texture != suppliedRenderTexture)
            {
                imageTextureDisplayer.texture = suppliedRenderTexture;
            }
            if (cameraToRender.targetTexture != suppliedRenderTexture)
            {
                cameraToRender.targetTexture = suppliedRenderTexture;
            }
            if (cameraToRender.targetTexture.width != Screen.width || cameraToRender.targetTexture.height != Screen.height)
            {
                suppliedRenderTexture.Release();
                suppliedRenderTexture.width = Screen.width;
                suppliedRenderTexture.height = Screen.height;
                cameraToRender.ResetAspect();
            }
        }
    }
}
