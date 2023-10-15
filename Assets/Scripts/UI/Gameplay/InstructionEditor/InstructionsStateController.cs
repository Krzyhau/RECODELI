using RecoDeli.Scripts.Gameplay.Robot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RecoDeli.Scripts.UI
{
    public class InstructionsStateController
    {

        public class State
        {
            public List<RobotInstruction> Instructions = new();
            public int ButtonBasedFocusPosition;
            public List<int> SelectionIndices = new();
        }

        public class StatesSlot
        {
            public List<State> Undos = new();
            public List<State> Redos = new();
        }

        public int MaxUndoHistory { get; set; }

        private Dictionary<int, StatesSlot> commandStateSlots = new();
        private InstructionEditor editor;

        public InstructionsStateController(InstructionEditor editor)
        {
            this.editor = editor;
        }

        public StatesSlot GetCurrentSlot()
        {
            if (!commandStateSlots.ContainsKey(editor.CurrentSlot))
            {
                commandStateSlots[editor.CurrentSlot] = new StatesSlot();
            }
            return commandStateSlots[editor.CurrentSlot];
        }

        public void StoreUndoOperation(int focusPosition)
        {
            var slot = GetCurrentSlot();
            slot.Undos.Add(new State
            {
                Instructions = editor.InstructionBars
                    .Select(bar => bar.Instruction.Clone() as RobotInstruction)
                    .ToList(),
                ButtonBasedFocusPosition = focusPosition,
                SelectionIndices = editor.InstructionBars
                    .Where(bar => bar.Selected)
                    .Select(bar => editor.InstructionBars.IndexOf(bar))
                    .ToList()
            });

            if (slot.Undos.Count > MaxUndoHistory)
            {
                slot.Undos.RemoveAt(0);
            }

            slot.Redos.Clear();
        }


        public void Undo()
        {
            if (!editor.ListDocument.enabledSelf) return;

            var slot = GetCurrentSlot();
            if (slot.Undos.Count == 0) return;
            var undo = slot.Undos.Last();

            var focusPos = undo.ButtonBasedFocusPosition;

            slot.Redos.Add(new State
            {
                Instructions = editor.InstructionBars
                    .Select(bar => bar.Instruction.Clone() as RobotInstruction)
                    .ToList(),
                ButtonBasedFocusPosition = focusPos,
                SelectionIndices = editor.InstructionBars
                    .Where(bar => bar.Selected)
                    .Select(bar => editor.InstructionBars.IndexOf(bar))
                    .ToList()
            });

            foreach (var bar in editor.InstructionBars.ToList())
            {
                editor.DeleteInstructionBar(bar);
            }

            var currentIndex = 0;
            foreach (var instruction in undo.Instructions)
            {
                var bar = editor.CreateInstructionBarAt(instruction, currentIndex);
                bar.Selected = undo.SelectionIndices.Contains(currentIndex);
                currentIndex++;
            }

            slot.Undos.RemoveAt(slot.Undos.Count - 1);

            editor.ScrollToInstruction(focusPos, true);

            editor.InstructionsListModified();
        }

        public void Redo()
        {
            if (!editor.ListDocument.enabledSelf) return;

            var slot = GetCurrentSlot();
            if (slot.Redos.Count == 0) return;
            var redo = slot.Redos.Last();
            var focusPos = redo.ButtonBasedFocusPosition;

            slot.Undos.Add(new State
            {
                Instructions = editor.InstructionBars.Select(bar => bar.Instruction.Clone() as RobotInstruction).ToList(),
                ButtonBasedFocusPosition = focusPos,
                SelectionIndices = editor.InstructionBars.Where(bar => bar.Selected).Select(bar => editor.InstructionBars.IndexOf(bar)).ToList()
            });

            foreach (var bar in editor.InstructionBars.ToList())
            {
                editor.DeleteInstructionBar(bar);
            }

            var currentIndex = 0;

            foreach (var instruction in redo.Instructions)
            {
                var bar = editor.CreateInstructionBarAt(instruction, currentIndex);
                bar.Selected = redo.SelectionIndices.Contains(currentIndex);
                currentIndex++;
            }

            slot.Redos.RemoveAt(slot.Redos.Count - 1);

            editor.ScrollToInstruction(focusPos, true);

            editor.InstructionsListModified();
        }
    }
}
