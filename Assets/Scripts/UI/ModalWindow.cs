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

        protected virtual void Awake()
        {
            closeButton = RootElement.Q<Button>("close-button");
            closeButton.clicked += () => Opened = false;


            RootElement.RegisterCallback<NavigationCancelEvent>(e => OnNavigationCancel(e));
            RootElement.RegisterCallback<NavigationMoveEvent>(e => OnNavigationMove(e));

            SetOpened(false);
        }

        private void OnNavigationCancel(NavigationCancelEvent evt)
        {
            SetOpened(false);
        }

        private void OnNavigationMove(NavigationMoveEvent evt)
        {
            var root = (evt.target as VisualElement).GetRootElement();
            if ((evt.target as VisualElement).GetRootElement() != RootElement)
            {
                evt.PreventDefault();
            }
        }

        public virtual void SetOpened(bool opened)
        {
            Opened = opened;
            if (opened)
            {
                RootElement.Focus();
            }
        }
    }
}
