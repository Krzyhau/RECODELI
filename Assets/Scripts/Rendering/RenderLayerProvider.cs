using UnityEngine;

namespace RecoDeli.Scripts.Rendering
{
    [ExecuteAlways]
    public abstract class RenderLayerProvider : MonoBehaviour
    {
        protected RenderTexture layer;

        [SerializeField] private UnityEngine.Experimental.Rendering.GraphicsFormat format;

        public RenderTexture RenderLayer => layer;

        private void OnEnable()
        {
            RefreshLayerTexture();

            if(layer != null)
            {
                AssignToRenderer(); 
            }
        }

        private void Update()
        {
            RefreshLayerTexture();

            AssignToRenderer();
            
        }

        protected void RefreshLayerTexture()
        {
            if (Screen.width == 0 || Screen.height == 0) return;

            if (layer == null)
            {
                layer = new RenderTexture(Screen.width, Screen.height, 1);
            }

            if (layer.graphicsFormat != format)
            {
                if (layer.IsCreated())
                {
                    layer.Release();
                }
                layer.graphicsFormat = format;
            }

            if (layer.width != Screen.width || layer.height != Screen.height)
            {
                if (layer.IsCreated())
                {
                    layer.Release();
                }
                layer.width = Screen.width;
                layer.height = Screen.height;
            }
        }

        protected virtual void AssignToRenderer() {

        }
    }
}
