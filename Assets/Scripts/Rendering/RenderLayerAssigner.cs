using UnityEngine;
using UnityEngine.UI;

namespace RecoDeli.Scripts.Rendering
{
    [ExecuteAlways]
    public class RenderLayerAssigner : MonoBehaviour
    {
        [SerializeField] private RenderLayerProvider provider;
        [SerializeField] private RawImage displayer;

        private void Update()
        {
            if(displayer.texture != provider.RenderLayer)
            {
                displayer.texture = provider.RenderLayer;
            }
        }
    }
}
