using UnityEngine;
using UnityEngine.UI;

namespace RecoDeli.Scripts.Rendering
{
    [ExecuteAlways]
    public class RenderLayerAssigner : MonoBehaviour
    {
        [SerializeField] private RenderLayerProvider provider;
        [SerializeField] private RawImage displayer;

        private void OnEnable()
        {
            RegisterEditorEvents();
        }

        private void OnDisable()
        {
            UnregisterEditorEvents();
        }

        private void Update()
        {
            if (displayer == null || provider == null) return;
            if(displayer.texture != provider.RenderLayer)
            {
                displayer.texture = provider.RenderLayer;
            }
        }

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
            // clear texture field when saving - it'll be reassigned when saving is done
            if(displayer != null)
            {
                displayer.texture = null;
            }
        }
#else
        private void RegisterEditorEvents() { }
        private void UnregisterEditorEvents() { }
#endif
    }
}
