using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    [ExecuteAlways]
    public class PanelRenderTextureSupplier : MonoBehaviour
    {
        [SerializeField] private PanelSettings linkedPanelSettings;
        [SerializeField] private RawImage imageTextureDisplayer;

        private RenderTexture suppliedRenderTexture;

        public void Update()
        {
            if(linkedPanelSettings == null || imageTextureDisplayer == null)
            {
                return;
            }

            if(suppliedRenderTexture == null)
            {
                suppliedRenderTexture = new RenderTexture(Screen.width, Screen.height, 1);
            }
            if(imageTextureDisplayer.texture != suppliedRenderTexture)
            {
                imageTextureDisplayer.texture = suppliedRenderTexture;
            }
            if(linkedPanelSettings.targetTexture != suppliedRenderTexture)
            {
                linkedPanelSettings.targetTexture = suppliedRenderTexture;
            }
            if(linkedPanelSettings.targetTexture.width != Screen.width || linkedPanelSettings.targetTexture.height != Screen.height)
            {
                suppliedRenderTexture.Release();
                suppliedRenderTexture.width = Screen.width;
                suppliedRenderTexture.height = Screen.height;
            }
        }
    }
}
