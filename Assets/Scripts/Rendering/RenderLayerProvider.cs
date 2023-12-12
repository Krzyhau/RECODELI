using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEditor;

namespace RecoDeli.Scripts.Rendering
{
    [ExecuteAlways]
    public abstract class RenderLayerProvider : MonoBehaviour
    {
        protected RenderTexture layer;

        [SerializeField] private GraphicsFormat format;

        public RenderTexture RenderLayer => layer;

        private void OnEnable()
        {
            RefreshLayerTexture();

            if(layer != null)
            {
                AssignToRenderer(); 
            }

            RegisterEditorEvents();
        }

        private void OnDisable()
        {
            UnregisterEditorEvents();
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
                layer.wrapMode = TextureWrapMode.Clamp;
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

        protected virtual void AssignToRenderer() {}

#if UNITY_EDITOR
        private void RegisterEditorEvents()
        {
            UnityEditor.SceneManagement.EditorSceneManager.sceneSaving += OnSceneSaving;
        }

        private void UnregisterEditorEvents()
        {
            UnityEditor.SceneManagement.EditorSceneManager.sceneSaving -= OnSceneSaving;
        }
        private void OnSceneSaving(UnityEngine.SceneManagement.Scene scene, string path)
        {
            // remove render texture when saving to prevent it from being stored in the scene file
            // it will be dynamically regenerated when used again
            if (layer == null) return;
            layer.Release();
            layer.DiscardContents();
        }
#else
        private void RegisterEditorEvents() { }
        private void UnregisterEditorEvents() { }
#endif
    }
}
