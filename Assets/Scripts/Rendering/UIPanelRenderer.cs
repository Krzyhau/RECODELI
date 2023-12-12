using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.Rendering
{
    [ExecuteAlways]
    public class UIPanelRenderer : RenderLayerProvider
    {
        [SerializeField] protected PanelSettings assignedPanel;
        protected override void AssignToRenderer()
        {
            base.AssignToRenderer();

            if (assignedPanel == null) return;

            if (assignedPanel.targetTexture != layer)
            {
                assignedPanel.targetTexture = layer;
            }
        }

        protected override void ClearRenderer()
        {
            base.ClearRenderer();

            if (assignedPanel == null) return;

            assignedPanel.targetTexture = null;
        }
    }
}
