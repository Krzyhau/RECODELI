using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts
{
    public class UISoundInjector : MonoBehaviour
    {
        [SerializeField] private AudioSource uiAudioSource;
        [SerializeField] private UIDocument[] documentsToInject;

        [System.Serializable]
        private enum SoundType 
        { 
            Press,
            Hover
        }

        [System.Serializable]
        private struct SoundAssignment
        {
            public string Classname;
            public SoundType Type;
            public AudioClip Clip;
        }

        [SerializeField] private AudioClip errorClip;

        [SerializeField] private List<SoundAssignment> soundAssignments;

        private HashSet<VisualElement> visualTreesToMonitor = new();

        private bool pressedOnActiveElement = false;

        private void Start()
        {
            // isolate individual trees from assigned UI documents
            foreach (var document in documentsToInject)
            {
                visualTreesToMonitor.Add(document.rootVisualElement.panel.visualTree);
            }

            foreach(var tree in visualTreesToMonitor)
            {
                tree.RegisterCallback<ClickEvent>(OnElementClick, TrickleDown.TrickleDown);
                tree.RegisterCallback<MouseEnterEvent>(OnElementHover, TrickleDown.TrickleDown);
                tree.RegisterCallback<NavigationMoveEvent>(OnElementNavigation, TrickleDown.TrickleDown);
                tree.RegisterCallback<NavigationSubmitEvent>(OnElementNavigationEnter, TrickleDown.TrickleDown);
                tree.RegisterCallback<PointerDownEvent>(OnElementPressed, TrickleDown.TrickleDown);
            }
        }

        private void OnDestroy()
        {
            foreach (var tree in visualTreesToMonitor)
            {
                tree.UnregisterCallback<ClickEvent>(OnElementClick, TrickleDown.TrickleDown);
                tree.UnregisterCallback<MouseEnterEvent>(OnElementHover, TrickleDown.TrickleDown);
                tree.UnregisterCallback<NavigationMoveEvent>(OnElementNavigation, TrickleDown.TrickleDown);
                tree.UnregisterCallback<PointerDownEvent>(OnElementPressed, TrickleDown.TrickleDown);
                tree.UnregisterCallback<NavigationSubmitEvent>(OnElementNavigationEnter, TrickleDown.TrickleDown);
            }
        }

        private void OnElementPressed(PointerDownEvent evt)
        {
            pressedOnActiveElement = false;
            if(evt.target is VisualElement element && element.enabledInHierarchy)
            {
                pressedOnActiveElement = true;
            }
        }

        private void OnElementClick(ClickEvent evt)
        {
            TryPlaySound(evt.target as VisualElement, SoundType.Press);
        }

        private void OnElementHover(MouseEnterEvent evt)
        {
            TryPlaySound(evt.target as VisualElement, SoundType.Hover);
        }

        private void OnElementNavigation(NavigationMoveEvent evt)
        {
            TryPlaySound(evt.target as VisualElement, SoundType.Hover);
        }

        private void OnElementNavigationEnter(NavigationSubmitEvent evt)
        {
            var element = evt.target as VisualElement;

            pressedOnActiveElement = element.enabledInHierarchy;

            TryPlaySound(element, SoundType.Press);
        }

        private void TryPlaySound(VisualElement element, SoundType type)
        {
            if (element == null) return;

            if (type == SoundType.Press && !pressedOnActiveElement)
            {
                if (!element.enabledSelf && element.parent.enabledInHierarchy)
                {
                    uiAudioSource.PlayOneShot(errorClip);
                }
                return;
            }
            if (type == SoundType.Hover && !element.enabledInHierarchy) return;

            var soundsToPlay = soundAssignments.Where(
                s => s.Type == type && element.ClassListContains(s.Classname)
            );

            if (soundsToPlay.Any())
            {
                uiAudioSource.PlayOneShot(soundsToPlay.FirstOrDefault().Clip);
            }
        }
    }
}
