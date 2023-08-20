using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class SaveManagementWindow : ModalWindow
    {
        [SerializeField] private InstructionEditor instructionEditor;

        private List<Button> saveSlotsButtons;

        protected override void Awake()
        {
            base.Awake();

            saveSlotsButtons = RootElement.Query<Button>("save-button").ToList();
            for (int i = 0; i < saveSlotsButtons.Count; i++)
            {
                var slotToLoad = i;
                saveSlotsButtons[i].SetEnabled(i != instructionEditor.CurrentSlot);
                saveSlotsButtons[slotToLoad].clicked += () => instructionEditor.LoadSaveSlot(slotToLoad);
            }
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
