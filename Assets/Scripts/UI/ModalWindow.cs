﻿using System;
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
        [SerializeField] protected UIDocument windowDocument;

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
            closeButton.clicked += Close;

            Close();
        }

        public virtual void Open()
        {
            Opened = true;

            if (Current != null && Current != this)
            {
                Current.Close();
            }
            Current = this;
            RootElement.Focus();
        }

        public virtual void Close()
        {
            Opened = false;
            if (Current == this)
            {
                Current = null;
            }
        }
    }
}
