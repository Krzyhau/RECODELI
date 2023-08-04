using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class SaveManagementWindow : MonoBehaviour
    {
        [SerializeField] private UIDocument saveManagementDocument;
        [SerializeField] private InstructionEditor instructionEditor;

        private Button closeButton;
        private List<Button> saveSlotsButtons;

        public bool Opened => saveManagementDocument.rootVisualElement.enabledSelf;

        private void Awake()
        {
            closeButton = saveManagementDocument.rootVisualElement.Q<Button>("close-button");
            closeButton.clicked += () => SetDisplay(false);

            saveSlotsButtons = saveManagementDocument.rootVisualElement.Query<Button>("save-button").ToList();
            for (int i = 0; i < saveSlotsButtons.Count; i++)
            {
                var slotToLoad = i;
                saveSlotsButtons[i].SetEnabled(i != instructionEditor.CurrentSlot);
                saveSlotsButtons[slotToLoad].clicked += () => instructionEditor.LoadSaveSlot(slotToLoad);
            }

            SetDisplay(false);
        }

        public void SetDisplay(bool state)
        {
            saveManagementDocument.rootVisualElement.SetEnabled(state);
        }

        public void Update()
        {
            for(int i = 0; i < saveSlotsButtons.Count; i++)
            {
                saveSlotsButtons[i].SetEnabled(i != instructionEditor.CurrentSlot);
            }
        }
    }
}
