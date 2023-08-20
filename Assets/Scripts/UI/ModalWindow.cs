using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine;
using RecoDeli.Scripts.Utils;

namespace RecoDeli.Scripts.UI
{
    public class ModalWindow : MonoBehaviour
    {
        [SerializeField] private UIDocument windowDocument;

        private Button closeButton;

        public bool Opened {
            get => windowDocument.rootVisualElement.enabledSelf;
            private set => windowDocument.rootVisualElement.SetEnabled(value);
        }

        public VisualElement RootElement => windowDocument.rootVisualElement;

        public static ModalWindow Current { get; private set; }
        public static bool AnyOpened => Current != null;

        protected virtual void Awake()
        {
            closeButton = RootElement.Q<Button>("close-button");
            closeButton.clicked += () => Opened = false;

            SetOpened(false);
        }

        public virtual void SetOpened(bool opened)
        {
            if (opened)
            {
                if (Current != null)
                {
                    Current.SetOpened(false);
                }
                RootElement.Focus();
            }
            else { 
                if(Current == this)
                {
                    Current = null;
                }
            }

            Opened = opened;
        }
    }
}
