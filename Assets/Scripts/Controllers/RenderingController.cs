using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.Controllers
{
    [ExecuteAlways]
    internal class RenderingController : MonoBehaviour
    {
        [SerializeField] private Camera gameRenderCamera;
        [SerializeField] private PanelSettings uiPanelSettings;

        [SerializeField] private RawImage imageTextureDisplayer;

        private RenderTexture gameRenderTargetTexture;
        private RenderTexture uiPanelTargetTexture;

        public void Update()
        {
            if (imageTextureDisplayer == null) return;

            SetupGameRendering();
            SetupUIRendering();
        }

        private void SetupGameRendering()
        {
            if (gameRenderCamera == null || imageTextureDisplayer == null)
            {
                return;
            }

            if (gameRenderTargetTexture == null)
            {
                gameRenderTargetTexture = new RenderTexture(Screen.width, Screen.height, 1);
                gameRenderTargetTexture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat;
                gameRenderTargetTexture.antiAliasing = 8;
            }

            if (gameRenderCamera.targetTexture != gameRenderTargetTexture)
            {
                gameRenderCamera.targetTexture = gameRenderTargetTexture;
            }
            if (gameRenderCamera.targetTexture.width != Screen.width || gameRenderCamera.targetTexture.height != Screen.height)
            {
                gameRenderTargetTexture.Release();
                gameRenderTargetTexture.width = Screen.width;
                gameRenderTargetTexture.height = Screen.height;
                gameRenderCamera.ResetAspect();
            }

            if (gameRenderTargetTexture && imageTextureDisplayer.texture != gameRenderTargetTexture)
            {
                imageTextureDisplayer.texture = gameRenderTargetTexture;
            }
        }

        private void SetupUIRendering()
        {
            if (uiPanelSettings == null || imageTextureDisplayer == null)
            {
                return;
            }

            if (uiPanelTargetTexture == null)
            {
                uiPanelTargetTexture = new RenderTexture(Screen.width, Screen.height, 1);
                // uiPanelTargetTexture.antiAliasing = 8;
            }
            if (uiPanelSettings.targetTexture != uiPanelTargetTexture)
            {
                uiPanelSettings.targetTexture = uiPanelTargetTexture;
            }
            if (uiPanelSettings.targetTexture.width != Screen.width || uiPanelSettings.targetTexture.height != Screen.height)
            {
                uiPanelTargetTexture.Release();
                uiPanelTargetTexture.width = Screen.width;
                uiPanelTargetTexture.height = Screen.height;
            }

            if (uiPanelTargetTexture && imageTextureDisplayer.material != null && imageTextureDisplayer.material.GetTexture("_UITex") != uiPanelTargetTexture)
            {
                imageTextureDisplayer.material.SetTexture("_UITex", uiPanelTargetTexture);
            }
        }
    }
}
