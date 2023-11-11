using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace RecoDeli.Scripts.Rendering
{
    [ExecuteAlways]
    public class BlurLayerAssigner : MonoBehaviour
    {
        private static int blurTextureId = Shader.PropertyToID("_BlurTex");

        [SerializeField] protected RenderLayerProvider layerToBlur;
        [SerializeField] protected int downscale;
        [SerializeField] protected Material blurMaterial;

        [SerializeField] private RawImage displayer;

        private RenderTexture downscaled;
        private RenderTexture blurred;

        private void LateUpdate()
        {
            if (
                layerToBlur == null || layerToBlur.RenderLayer == null ||
                blurMaterial == null || displayer == null ||
                displayer.material == null || !displayer.material.HasTexture(blurTextureId)
            )
            {
                return;
            }

            if(downscaled == null)
            {
                downscaled = new RenderTexture(
                    layerToBlur.RenderLayer.width / downscale, 
                    layerToBlur.RenderLayer.height / downscale,
                    1
                );
                downscaled.graphicsFormat = layerToBlur.RenderLayer.graphicsFormat;
                downscaled.wrapMode = TextureWrapMode.Clamp;
            }

            if(blurred == null)
            {
                blurred = new RenderTexture(downscaled.width, downscaled.height, 1);
                blurred.graphicsFormat = downscaled.graphicsFormat;
                blurred.wrapMode = TextureWrapMode.Clamp;
            }

            if (
                downscaled.width != layerToBlur.RenderLayer.width / downscale ||
                downscaled.height != layerToBlur.RenderLayer.height / downscale ||
                downscaled.graphicsFormat != layerToBlur.RenderLayer.graphicsFormat
            ) {
                if (downscaled.IsCreated())
                {
                    downscaled.Release();
                }
                downscaled.width = layerToBlur.RenderLayer.width / downscale;
                downscaled.height = layerToBlur.RenderLayer.height / downscale;
                downscaled.graphicsFormat = layerToBlur.RenderLayer.graphicsFormat;

                if (blurred.IsCreated())
                {
                    blurred.Release();
                }
                blurred.width = downscaled.width;
                blurred.height = downscaled.height;
                blurred.graphicsFormat = downscaled.graphicsFormat;
            }

            if(displayer.material != null && displayer.material.GetTexture(blurTextureId) != blurred)
            {
                displayer.material.SetTexture(blurTextureId, blurred);
            }

            Profiler.BeginSample("BlurLayerRenderer - Downscaling");
            Graphics.Blit(layerToBlur.RenderLayer, downscaled);
            Profiler.EndSample();

            Profiler.BeginSample("BlurLayerRenderer - Blurring");
            Graphics.Blit(downscaled, blurred, blurMaterial);
            Profiler.EndSample();
        }
    }
}
