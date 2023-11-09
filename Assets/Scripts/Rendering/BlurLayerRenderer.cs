using UnityEngine;
using UnityEngine.Rendering;

namespace RecoDeli.Scripts.Rendering
{
    [ExecuteAlways]
    public class BlurLayerRenderer : RenderLayerProvider
    {
        [SerializeField] protected RenderLayerProvider backgroundLayer;
        [SerializeField] protected RenderLayerProvider foregroundMaskLayer;
        [SerializeField] protected int downscale;
        [SerializeField] protected Material blurMaterial;
        [SerializeField] protected Material combineMaterial;

        private Material combineMaterialInstance;
        private RenderTexture downscaledBackground;
        private RenderTexture blurredBackground;

        protected override void AssignToRenderer()
        {
            if (
                backgroundLayer == null || foregroundMaskLayer == null ||
                blurMaterial == null || combineMaterial == null ||
                backgroundLayer.RenderLayer == null || foregroundMaskLayer.RenderLayer == null
            )
            {
                return;
            }

            if(downscaledBackground == null)
            {
                downscaledBackground = new RenderTexture(
                    backgroundLayer.RenderLayer.width / downscale, 
                    backgroundLayer.RenderLayer.height / downscale,
                    1
                );
                downscaledBackground.graphicsFormat = backgroundLayer.RenderLayer.graphicsFormat;
            }

            if(blurredBackground == null)
            {
                blurredBackground = new RenderTexture(downscaledBackground.width, downscaledBackground.height, 1);
                blurredBackground.graphicsFormat = downscaledBackground.graphicsFormat;
            }

            if (
                downscaledBackground.width != backgroundLayer.RenderLayer.width / downscale ||
                downscaledBackground.height != backgroundLayer.RenderLayer.height / downscale ||
                downscaledBackground.graphicsFormat != backgroundLayer.RenderLayer.graphicsFormat
            ) {
                downscaledBackground.Release();
                downscaledBackground.width = backgroundLayer.RenderLayer.width / downscale;
                downscaledBackground.height = backgroundLayer.RenderLayer.height / downscale;
                downscaledBackground.graphicsFormat = backgroundLayer.RenderLayer.graphicsFormat;

                blurredBackground.Release();
                blurredBackground.width = downscaledBackground.width;
                blurredBackground.height = downscaledBackground.height;
                blurredBackground.graphicsFormat = downscaledBackground.graphicsFormat;
            }

            if(combineMaterialInstance == null && combineMaterial != null)
            {
                combineMaterialInstance = new Material(combineMaterial);
            }

            if(combineMaterialInstance != null && combineMaterialInstance.GetTexture("_ForegroundMaskTex") != foregroundMaskLayer.RenderLayer)
            {
                combineMaterialInstance.SetTexture("_ForegroundMaskTex", foregroundMaskLayer.RenderLayer);
            }

            Graphics.Blit(backgroundLayer.RenderLayer, downscaledBackground);
            Graphics.Blit(downscaledBackground, blurredBackground, blurMaterial);
            Graphics.Blit(blurredBackground, layer, combineMaterialInstance);
        }
    }
}
