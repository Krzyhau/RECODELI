using RecoDeli.Scripts.Controllers;
using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.SaveManagement;
using RecoDeli.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class InstructionEditor : MonoBehaviour
    {
        private class EditorCommandState
        {
            public List<RobotInstruction> Instructions = new();
            public int ButtonBasedFocusPosition;
            public List<int> SelectionIndices = new();
        }

        private class EditorCommandStatesSlot
        {
            public List<EditorCommandState> Undos = new();
            public List<EditorCommandState> Redos = new();
        }


        [SerializeField] private AddInstructionMenu addInstructionMenu;

        [Header("Settings")]
        [SerializeField] private int maxUndoHistory;
        [SerializeField] private float focusZoneEdgesSize;
        [SerializeField] private float grabScrollMaxSpeed;

        private VisualElement instructionEditorContainer;
        private ScrollView instructionsContainer;

        private Button addButton;
        private Button deleteButton;
        private Button undoButton;
        private Button redoButton;
        private Button copyButton;
        private Button pasteButton;

        private List<InstructionBar> instructionBars = new();

        private Dictionary<int, EditorCommandStatesSlot> commandStateSlots = new();
        private int currentSlot = 0;

        private List<InstructionBar> grabbedInstructions = new();
        private bool grabbing;
        private InstructionBar lastClickedInstructionBar;
        private bool playingInstructions;
        private float prePlayScrollPosition;

        private bool mouseOverList;
        private bool mouseHeldOnList;
        private float mouseListBasedPosition;

        public bool Grabbing => grabbing;
        public List<InstructionBar> InstructionBars => instructionBars;
        public int CurrentSlot => currentSlot;
        public int SkipToInstruction { get; private set; }

        public void Initialize(UIDocument gameDocument)
        {
            instructionEditorContainer = gameDocument.rootVisualElement.Q<VisualElement>("instruction-editor");
            instructionsContainer = gameDocument.rootVisualElement.Q<ScrollView>("instructions");

            addButton = gameDocument.rootVisualElement.Q<Button>("add-button");
            deleteButton = gameDocument.rootVisualElement.Q<Button>("delete-button");
            undoButton = gameDocument.rootVisualElement.Q<Button>("undo-button");
            redoButton = gameDocument.rootVisualElement.Q<Button>("redo-button");
            copyButton = gameDocument.rootVisualElement.Q<Button>("copy-button");
            pasteButton = gameDocument.rootVisualElement.Q<Button>("paste-button");

            addButton.clicked += () => { addInstructionMenu.Opened = true; };
            deleteButton.clicked += DeleteSelected;
            undoButton.clicked += Undo;
            redoButton.clicked += Redo;
            copyButton.clicked += CopySelected;
            pasteButton.clicked += Paste;

            instructionsContainer.contentContainer.RegisterCallback<MouseMoveEvent>((e) => OnMouseOverList(e));
            instructionsContainer.contentContainer.RegisterCallback<MouseLeaveEvent>((e) => OnMouseLeaveList(e));

            addInstructionMenu.Initialize(this, gameDocument);
        }

        private void Update()
        {
            UpdateBarsSelection();
            HandleBarGrabbing();

            HandleKeyboardShortcuts();

            UpdateEditorButtonsState();

            foreach (var instructionBar in instructionBars)
            {
                instructionBar.Update();
            }

            instructionEditorContainer.EnableInClassList("no-instructions", instructionBars.Count == 0);
        }

        private void OnMouseOverList(MouseMoveEvent evt)
        {
            mouseOverList = true;
         
            // calculate list-based mouse position (aka. over which element vertically mouse currently hovers)

            var yPosition = Mathf.Max(0.0f, evt.localMousePosition.y - instructionsContainer.contentContainer.resolvedStyle.paddingTop);

            var currentPosition = 0.0f;
            var buttonIndex = 0;

            foreach (var bar in instructionBars)
            {
                var barStyle = bar.resolvedStyle;
                var height = barStyle.marginTop + barStyle.marginBottom + barStyle.height;

                if (yPosition >= currentPosition && yPosition < currentPosition + height)
                {
                    float innerFactor = (yPosition - currentPosition) / height;
                    mouseListBasedPosition = buttonIndex + innerFactor;
                    return;
                }

                currentPosition += height;
                buttonIndex++;
            }

            mouseListBasedPosition = buttonIndex;
        }

        private void OnMouseLeaveList(MouseLeaveEvent evt)
        {
            mouseOverList = false;
            // keep the last list-based position so that grabbing doesn't do anything weird
        }

        private void UpdateBarsSelection()
        {
            if (addInstructionMenu.Opened)
            {
                return;
            }

            // during playback, only set skipping functionality
            if (playingInstructions)
            {
                for(int i = 0; i <= SkipToInstruction; i++)
                {
                    instructionBars[i].Selected = false;
                }

                if (Input.GetMouseButtonDown(0) && mouseOverList)
                {
                    InstructionBar clickedBar = instructionBars
                    .Where(bar => bar.IsPointerHoveringOnHandle())
                    .FirstOrDefault();

                    foreach (var bar in instructionBars)
                    {
                        bar.Selected = false;
                    }

                    if(clickedBar != null && clickedBar.Instruction.Progress == 0.0f)
                    {
                        clickedBar.Selected = true;
                        SkipToInstruction = instructionBars.IndexOf(clickedBar);
                    }
                }

                return;
            }
            else
            {
                SkipToInstruction = -1;
            }

            bool multipleSelection = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool rangeSelection = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (Input.GetMouseButtonDown(0) && mouseOverList)
            {
                InstructionBar clickedBar = instructionBars
                    .Where(bar => bar.IsPointerHoveringOnHandle())
                    .FirstOrDefault();

                // unselect all when needed
                if ((clickedBar == null || !clickedBar.Selected) && !multipleSelection && !rangeSelection)
                {
                    foreach (var bar in instructionBars)
                    {
                        bar.Selected = false;
                    }
                }

                // find out what to select
                if (clickedBar != null)
                {
                    if (lastClickedInstructionBar == null || !lastClickedInstructionBar.Selected)
                    {
                        lastClickedInstructionBar = instructionBars.Where(bar => bar.Selected).FirstOrDefault();
                    }

                    if (rangeSelection && lastClickedInstructionBar != null)
                    {
                        int startPos = instructionBars.IndexOf(lastClickedInstructionBar);
                        int endPos = instructionBars.IndexOf(clickedBar);
                        int step = startPos > endPos ? -1 : 1;
                        for (int i = startPos; i != endPos; i += step)
                        {
                            if (i < 0 || i >= instructionBars.Count) break;
                            instructionBars[i].Selected = true;
                        }
                    }

                    if (multipleSelection || !clickedBar.Selected)
                    {
                        clickedBar.Selected = !clickedBar.Selected;
                    }

                    lastClickedInstructionBar = clickedBar;
                }
                else
                {
                    lastClickedInstructionBar = null;
                }
            }

            // unselect other buttons
            if (!playingInstructions && Input.GetMouseButtonUp(0) && !grabbing && !rangeSelection && !multipleSelection && mouseOverList)
            {
                foreach (var bar in instructionBars)
                {
                    if (bar != lastClickedInstructionBar) bar.Selected = false;
                }
            }

        }

        private void HandleBarGrabbing()
        {
            if (addInstructionMenu.Opened || playingInstructions)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                mouseHeldOnList = mouseOverList;
            }

            if (
                Input.GetMouseButton(0) && !grabbing && mouseHeldOnList &&
                lastClickedInstructionBar != null && lastClickedInstructionBar.Selected
            )
            {
                if (!lastClickedInstructionBar.IsPointerHoveringOnHandle())
                {
                    var selectedBars = instructionBars.Where(bar => bar.Selected);

                    if (selectedBars.Any())
                    {
                        grabbing = true;
                        grabbedInstructions = selectedBars.ToList();
                        StoreUndoOperation(Mathf.RoundToInt((float)selectedBars.Select(bar => instructionBars.IndexOf(bar)).Average()));
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && grabbing)
            {
                grabbing = false;
                grabbedInstructions.Clear();
                mouseHeldOnList = false;
                InstructionsListModified();
            }

            if (grabbing)
            {
                int newPos = Mathf.FloorToInt(mouseListBasedPosition);
                int oldPos = instructionBars.IndexOf(lastClickedInstructionBar);

                int minPos = newPos - grabbedInstructions.IndexOf(lastClickedInstructionBar);
                int maxPos = minPos + grabbedInstructions.Count - 1;

                if (newPos != oldPos && minPos >= 0 && maxPos < instructionsContainer.childCount)
                {
                    // temporarily "remove" grabbed instructions so they can be moved
                    foreach (var bar in grabbedInstructions)
                    {
                        instructionsContainer.Remove(bar);
                        instructionBars.Remove(bar);
                    }

                    int currPos = minPos;

                    foreach (var bar in grabbedInstructions)
                    {
                        instructionsContainer.Insert(currPos, bar);
                        instructionBars.Insert(currPos, bar);
                        currPos++;
                    }
                }

                // scroll list up or down if near the edge
                var mousePosition = Input.mousePosition;
                mousePosition.y = Screen.height - mousePosition.y;
                var mouseLocalPosition = instructionsContainer.WorldToLocal(
                    RuntimePanelUtils.ScreenToPanel(instructionsContainer.panel, mousePosition)
                );

                var halfListHeight = instructionsContainer.localBound.height * 0.5f;
                var midDistance = mouseLocalPosition.y - halfListHeight;
                var edgeSpeed = Math.Clamp((Math.Abs(midDistance) - (halfListHeight - focusZoneEdgesSize)), 0.0f, 1.0f);
                edgeSpeed *= Math.Sign(midDistance) / focusZoneEdgesSize;

                if(edgeSpeed != 0.0f)
                {
                    instructionsContainer.scrollOffset += Vector2.up * edgeSpeed * grabScrollMaxSpeed;
                }
            }
        }

        private void UpdateEditorButtonsState()
        {
            if (addInstructionMenu.Opened || playingInstructions)
            {
                addButton.SetEnabled(false);
                deleteButton.SetEnabled(false);
                undoButton.SetEnabled(false);
                redoButton.SetEnabled(false);
                copyButton.SetEnabled(false);
                pasteButton.SetEnabled(false);
            }
            else
            {
                addButton.SetEnabled(!playingInstructions);
                deleteButton.SetEnabled(instructionBars.Where(bar => bar.Selected).Any());
                undoButton.SetEnabled(GetCurrentCommandStateSlot().Undos.Count > 0);
                redoButton.SetEnabled(GetCurrentCommandStateSlot().Redos.Count > 0);
                copyButton.SetEnabled(deleteButton.enabledSelf);
                pasteButton.SetEnabled(RobotInstruction.IsValidListString(GUIUtility.systemCopyBuffer));
            }
        }

        private void HandleKeyboardShortcuts()
        {
            if (!addInstructionMenu.Opened && !playingInstructions && !EventSystem.current.HasInputFieldSelected())
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    SetAllBarsSelected(true);
                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    CopySelected();
                }
                else if (Input.GetKeyDown(KeyCode.V))
                {
                    Paste();
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    Undo();
                }
                else if (Input.GetKeyDown(KeyCode.Y))
                {
                    Redo();
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SetAllBarsSelected(false);
                }
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    DeleteSelected();
                }
            }
            else if(addInstructionMenu.Opened)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    addInstructionMenu.Opened = false;
                }
            }
        }


        private InstructionBar CreateInstructionBarAt(RobotInstruction instruction, int index)
        {
            if (instruction == null)
            {
                return null;
            }

            var bar = new InstructionBar();

            bar.Selected = true;
            bar.Instruction = instruction.Clone() as RobotInstruction;
            bar.changing += () => StoreUndoOperation(index);
            bar.changed += () => InstructionsListModified();

            instructionsContainer.Insert(index, bar);
            instructionBars.Insert(index, bar);

            return bar;
        }

        private void DeleteInstructionBar(InstructionBar bar)
        {
            instructionsContainer.Remove(bar);
            instructionBars.Remove(bar);
        }

        private void SetAllBarsSelected(bool selected)
        {
            foreach(var bar in instructionBars)
            {
                bar.Selected = selected;
            }
        }

        private EditorCommandStatesSlot GetCurrentCommandStateSlot()
        {
            if (!commandStateSlots.ContainsKey(currentSlot))
            {
                commandStateSlots[currentSlot] = new EditorCommandStatesSlot();
            }
            return commandStateSlots[currentSlot];
        }

        private void StoreUndoOperation(int focusPosition)
        {
            var slot = GetCurrentCommandStateSlot();
            slot.Undos.Add(new EditorCommandState
            {
                Instructions = instructionBars.Select(bar => bar.Instruction.Clone() as RobotInstruction).ToList(),
                ButtonBasedFocusPosition = focusPosition,
                SelectionIndices = instructionBars.Where(bar => bar.Selected).Select(bar => instructionBars.IndexOf(bar)).ToList()
            });

            if (slot.Undos.Count > maxUndoHistory)
            {
                slot.Undos.RemoveAt(0);
            }

            slot.Redos.Clear();
        }

        public void AddInstructions(List<RobotInstruction> instructions, bool quiet = false)
        {
            var addIndex = instructionsContainer.childCount - 1;
            var selected = instructionBars.Where(bar => bar.Selected);
            if (selected.Any())
            {
                addIndex = selected.Select(bar => instructionBars.IndexOf(bar)).Max();
            }

            if (!quiet)
            {
                StoreUndoOperation(addIndex + instructions.Count / 2);
            }

            foreach (var selectedBar in selected)
            {
                selectedBar.Selected = false;
            }

            foreach (RobotInstruction instruction in instructions)
            {
                CreateInstructionBarAt(instruction, addIndex + 1);

                addIndex++;
            }
            addInstructionMenu.Opened = false;

            if (!quiet)
            {
                InstructionsListModified();
            }
        }

        public void AddInstruction(RobotInstruction instruction, bool quiet = false)
        {
            AddInstructions(new List<RobotInstruction>() { instruction }, quiet);
        }

        private void InstructionsListModified()
        {
            var levelInfo = SaveManager.CurrentSave.GetLevelInfo(LevelLoader.CurrentlyLoadedLevel);

            levelInfo.Slots[currentSlot].Instructions = GetRobotInstructionsList();
        }

        public void LoadSaveSlot(int slot)
        {
            foreach (var bar in instructionBars.ToList())
            {
                DeleteInstructionBar(bar);
            }

            currentSlot = slot;

            var saveLevelData = SaveManager.CurrentSave.GetLevelInfo(LevelLoader.CurrentlyLoadedLevel);
            if (saveLevelData != null)
            {
                AddInstructions(saveLevelData.Slots[currentSlot].Instructions, true);
                SetAllBarsSelected(false);
            }
        }

        public void DeleteSelected()
        {
            var selectedBars = instructionBars.Where(bar => bar.Selected).ToList();

            if (selectedBars.Any())
            {
                var barToSelectAfterwards = Math.Max(0, selectedBars.Select(bar => instructionBars.IndexOf(bar)).Min());

                var focusIndex = selectedBars.Select(bar => instructionBars.IndexOf(bar)).Average();
                StoreUndoOperation(Mathf.RoundToInt((float)focusIndex));

                foreach (var bar in selectedBars)
                {
                    DeleteInstructionBar(bar);
                }

                if (barToSelectAfterwards >= 0 && instructionBars.Count > 0)
                {
                    barToSelectAfterwards = Math.Min(barToSelectAfterwards, instructionBars.Count - 1);
                    instructionBars[barToSelectAfterwards].Selected = true;
                }

                InstructionsListModified();
            }
        }

        public void Undo()
        {
            var slot = GetCurrentCommandStateSlot();
            if (slot.Undos.Count == 0) return;
            var undo = slot.Undos.Last();

            var focusPos = undo.ButtonBasedFocusPosition;

            slot.Redos.Add(new EditorCommandState
            {
                Instructions = instructionBars.Select(bar => bar.Instruction.Clone() as RobotInstruction).ToList(),
                ButtonBasedFocusPosition = focusPos,
                SelectionIndices = instructionBars.Where(bar => bar.Selected).Select(bar => instructionBars.IndexOf(bar)).ToList()
            });

            foreach (var bar in instructionBars.ToList())
            {
                DeleteInstructionBar(bar);
            }

            var currentIndex = 0;
            foreach (var instruction in undo.Instructions)
            {
                var bar = CreateInstructionBarAt(instruction, currentIndex);
                bar.Selected = undo.SelectionIndices.Contains(currentIndex);
                currentIndex++;
            }

            slot.Undos.RemoveAt(slot.Undos.Count - 1);

            grabbing = false;

            FocusOnInstruction(focusPos);

            InstructionsListModified();
        }

        public void Redo()
        {
            var slot = GetCurrentCommandStateSlot();
            if (slot.Redos.Count == 0) return;
            var redo = slot.Redos.Last();
            var focusPos = redo.ButtonBasedFocusPosition;

            slot.Undos.Add(new EditorCommandState
            {
                Instructions = instructionBars.Select(bar => bar.Instruction.Clone() as RobotInstruction).ToList(),
                ButtonBasedFocusPosition = focusPos,
                SelectionIndices = instructionBars.Where(bar => bar.Selected).Select(bar => instructionBars.IndexOf(bar)).ToList()
            });

            foreach (var bar in instructionBars.ToList())
            {
                DeleteInstructionBar(bar);
            }

            var currentIndex = 0;

            foreach (var instruction in redo.Instructions)
            {
                var bar = CreateInstructionBarAt(instruction, currentIndex);
                bar.Selected = redo.SelectionIndices.Contains(currentIndex);
                currentIndex++;
            }

            slot.Redos.RemoveAt(slot.Redos.Count - 1);

            grabbing = false;

            FocusOnInstruction(focusPos);

            InstructionsListModified();
        }

        public void CopySelected()
        {
            var selectedBars = instructionBars.Where(bar => bar.Selected);

            if (selectedBars.Any())
            {
                GUIUtility.systemCopyBuffer = RobotInstruction.ListToString(
                    selectedBars.Select(bar => bar.Instruction).ToList()
                );
            }
        }

        public void Paste()
        {
            var clipboard = GUIUtility.systemCopyBuffer;
            if (RobotInstruction.IsValidListString(clipboard))
            {
                AddInstructions(RobotInstruction.StringToList(clipboard));
            }
        }

        public void FocusOnInstruction(int instructionIndex)
        {
            if (instructionBars.Count == 0) return;
            var bar = instructionBars[Math.Clamp(instructionIndex, 0, instructionBars.Count - 1)];

            // Bars that have been created this frame might not have been
            // fully recalculated yet. Delay setting scroll offset until
            // the UI Toolkit is done with recalculations, so it isn't just set to 0.
            StartCoroutine(FocusOnInstructionBarCoroutine(bar));
        }

        private IEnumerator FocusOnInstructionBarCoroutine(InstructionBar bar)
        {
            // wait one frame. awful, but what can I do?!
            yield return null;

            // make instruction appear in the middle of the scrolling container
            var barPos = bar.localBound.y;
            var targetYScroll = barPos - instructionsContainer.localBound.height * 0.5f;

            instructionsContainer.scrollOffset = Vector2.up * targetYScroll;
        }

        public void HighlightInstruction(int instructionIndex)
        {
            if (instructionsContainer == null) return;

            if (instructionIndex == -1)
            {
                instructionsContainer.scrollOffset = Vector2.up * prePlayScrollPosition;
            }
            else
            {
                FocusOnInstruction(instructionIndex);
            }
        }

        public void SetPlaybackState(bool state)
        {
            playingInstructions = state;

            if (instructionsContainer == null) return;

            if (!state)
            {
                foreach (var bar in instructionBars)
                {
                    bar.Instruction.UpdateProgress(0.0f);
                }
            }

            instructionsContainer.contentContainer.SetEnabled(!state);

            if (state)
            {
                addInstructionMenu.Opened = false;
                prePlayScrollPosition = instructionsContainer.scrollOffset.y;
                foreach (var bar in instructionBars)
                {
                    bar.Selected = false;
                }
            }
        }

        public List<RobotInstruction> GetRobotInstructionsList()
        {
            return instructionBars.Select(bar => bar.Instruction).ToList();
        }
    }
}
