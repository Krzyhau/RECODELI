using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RecoDeli.Scripts.Prototyping
{
    public class RotationMethodSelector : MonoBehaviour
    {
        [SerializeField] private ToggleGroup toggleGroup;

        public static bool ShouldUseFreeMethod { get;private set; }
        public static bool ShouldUseDragMethod { get;private set; }

        private void Update()
        {
            var id = toggleGroup.GetFirstActiveToggle().transform.GetSiblingIndex();
            ShouldUseFreeMethod = id == 1;
            ShouldUseDragMethod = id == 2;
        }
    }
}
