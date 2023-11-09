using UnityEngine;
using UnityEngine.Rendering;
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

        [SerializeField] private Material downsampleMaterial;
        [SerializeField] private int downsampleRatio;

        private RenderTexture gameRenderTargetTexture;
        private RenderTexture gameRenderDownsampledTextureBuffer;
        private RenderTexture gameRenderDownsampledTexture;
        private RenderTexture uiPanelTargetTexture;

        public void OnEnable()
        {
            RenderPipelineManager.endCameraRendering += OnPostGameCameraRender;
        }

        public void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= OnPostGameCameraRender;
        }

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
                // gameRenderTargetTexture.antiAliasing = 8;
            }
            if(gameRenderDownsampledTexture == null)
            {
                gameRenderDownsampledTexture = new RenderTexture(Screen.width / downsampleRatio, Screen.height / downsampleRatio, 1);
                gameRenderDownsampledTexture.graphicsFormat = gameRenderTargetTexture.graphicsFormat;
            }
            if(gameRenderDownsampledTextureBuffer == null)
            {
                gameRenderDownsampledTextureBuffer = new RenderTexture(Screen.width / downsampleRatio, Screen.height / downsampleRatio, 1);
                gameRenderDownsampledTextureBuffer.graphicsFormat = gameRenderTargetTexture.graphicsFormat;
            }

            if (gameRenderCamera.targetTexture != gameRenderTargetTexture)
            {
                gameRenderCamera.targetTexture = gameRenderTargetTexture;
            }

            if (gameRenderCamera.targetTexture.width != Screen.width || gameRenderCamera.targetTexture.height != Screen.height)
            {
                gameRenderTargetTexture.Release();
                gameRenderDownsampledTexture.Release();
                gameRenderDownsampledTextureBuffer.Release();
                gameRenderTargetTexture.width = Screen.width;
                gameRenderTargetTexture.height = Screen.height;
                gameRenderDownsampledTexture.width = Screen.width / downsampleRatio;
                gameRenderDownsampledTexture.height = Screen.height / downsampleRatio;
                gameRenderDownsampledTextureBuffer.width = gameRenderDownsampledTexture.width;
                gameRenderDownsampledTextureBuffer.height = gameRenderDownsampledTexture.width;
                gameRenderCamera.ResetAspect();
            }

            if (gameRenderTargetTexture && imageTextureDisplayer.texture != gameRenderTargetTexture)
            {
                imageTextureDisplayer.texture = gameRenderTargetTexture;
                
            }

            if (gameRenderDownsampledTexture && imageTextureDisplayer.material != null && 
                imageTextureDisplayer.material.GetTexture("_MainTexDownsampled") != gameRenderDownsampledTexture)
            {
                imageTextureDisplayer.material.SetTexture("_MainTexDownsampled", gameRenderDownsampledTexture);
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

        private void OnPostGameCameraRender(ScriptableRenderContext context, Camera cam)
        {
            if (cam != gameRenderCamera) return;
            if (gameRenderDownsampledTexture == null || gameRenderTargetTexture == null || downsampleMaterial == null) return;
            Graphics.Blit(gameRenderTargetTexture, gameRenderDownsampledTextureBuffer);
            Graphics.Blit(gameRenderDownsampledTextureBuffer, gameRenderDownsampledTexture, downsampleMaterial);
        }
    }
}
