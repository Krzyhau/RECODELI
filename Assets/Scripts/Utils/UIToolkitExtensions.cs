using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.Utils
{
    public static class UIToolkitExtensions
    {
        public static VisualElement GetRootElement(this VisualElement element)
        {
            var root = element;
            while(root.parent != null)
            {
                root = root.parent;
            }

            return root;
        }
    }
}
