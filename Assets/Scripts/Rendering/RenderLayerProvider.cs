using UnityEngine;

namespace RecoDeli.Scripts.Rendering
{
    [ExecuteAlways]
    public abstract class RenderLayerProvider : MonoBehaviour
    {
        protected RenderTexture layer;

        [SerializeField] private UnityEngine.Experimental.Rendering.GraphicsFormat format;

        public RenderTexture RenderLayer => layer;

        private void Update()
        {
            if (layer == null)
            {
                layer = new RenderTexture(Screen.width, Screen.height, 1);
            }

            if(layer.graphicsFormat != format)
            {
                layer.Release();
                layer.graphicsFormat = format;
            }

            if (layer.width != Screen.width || layer.height != Screen.height)
            {
                layer.Release();
                layer.width = Screen.width;
                layer.height = Screen.height;
            }

            AssignToRenderer();
            
        }

        protected virtual void AssignToRenderer() {

        }
    }
}
