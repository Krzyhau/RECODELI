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
            Release,
            Hover
        }

        [System.Serializable]
        private struct SoundAssignment
        {
            public string Classname;
            public SoundType Type;
            public AudioClip Clip;
        }

        [SerializeField] private List<SoundAssignment> soundAssignments;

        private HashSet<VisualElement> visualTreesToMonitor = new();

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
            }
        }

        private void OnDestroy()
        {
            foreach (var tree in visualTreesToMonitor)
            {
                tree.UnregisterCallback<ClickEvent>(OnElementClick, TrickleDown.TrickleDown);
            }
        }

        private void OnElementClick(ClickEvent evt)
        {
            var element = evt.target as VisualElement;

            var soundsToPlay = soundAssignments.Where(
                s => s.Type == SoundType.Press && element.ClassListContains(s.Classname)
            );

            foreach (var sound in soundsToPlay)
            {
                Debug.Log($"{uiAudioSource}, {sound.Clip}, {sound.Classname}");
                if (sound.Clip == null) continue;
                uiAudioSource.PlayOneShot(sound.Clip);
            }
        }
    }
}
