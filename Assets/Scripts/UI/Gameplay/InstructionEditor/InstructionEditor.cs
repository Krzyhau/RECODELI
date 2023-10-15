using RecoDeli.Scripts.Controllers;
using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.SaveManagement;
using RecoDeli.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


namespace RecoDeli.Scripts.UI
{
    public class InstructionEditor : MonoBehaviour
    {
        [SerializeField] private SimulationInterface simulationInterface;
        [SerializeField] private AddInstructionMenu addInstructionMenu;

        [Header("Settings")]
        [SerializeField] private int maxUndoHistory;
        [SerializeField] private float focusZoneEdgesSize;
        [SerializeField] private float grabScrollMaxSpeed;
        [SerializeField] private float doubleClickMaxDelay;

        [Header("Buttons")]
        [SerializeField] private InputActionReference addInput;
        [SerializeField] private InputActionReference replaceInput;
        [SerializeField] private InputActionReference deleteInput;
        [SerializeField] private InputActionReference undoInput;
        [SerializeField] private InputActionReference redoInput;
        [SerializeField] private InputActionReference copyInput;
        [SerializeField] private InputActionReference pasteInput;
        [SerializeField] private InputActionReference selectAllInput;
        [SerializeField] private InputActionReference multiSelectionInput;
        [SerializeField] private InputActionReference rangeSelectionInput;

        private VisualElement instructionEditorContainer;
        private ScrollView instructionsContainer;

        private Button addButton;
        private Button deleteButton;
        private Button undoButton;
        private Button redoButton;
        private Button copyButton;
        private Button pasteButton;

        private List<InstructionBar> instructionBars = new();

        private InstructionsStateController commandStateController;
        private int currentSlot = 0;

        private bool grabbing;
        private InstructionBar currentlyHeldBar;
        private InstructionBar lastFocusedInstructionBar;
        private bool doNotUnselectNextFocus;
        private float lastClickedInstructionBarTime;
        private bool playingInstructions;
        private float prePlayScrollPosition;

        private float mouseListBasedPosition;
        private float mouseListRealPosition;
        private int remainingRepositioningRequests;

        public bool Grabbing => grabbing;
        public List<InstructionBar> InstructionBars => instructionBars;
        public int CurrentSlot => currentSlot;
        public int SkipToInstruction { get; private set; }
        public AddInstructionMenu AddInstructionMenu => addInstructionMenu;

        public ScrollView ListDocument => instructionsContainer;
        public VisualElement EditorDocument => instructionEditorContainer;

        public void Initialize(UIDocument gameDocument)
        {
            commandStateController = new(this);
            commandStateController.MaxUndoHistory = maxUndoHistory;

            InitializeInterface(gameDocument);
            InitializeInputs();
        }

        private void InitializeInterface(UIDocument gameDocument)
        {
            instructionEditorContainer = gameDocument.rootVisualElement.Q<VisualElement>("instruction-editor");
            instructionsContainer = gameDocument.rootVisualElement.Q<ScrollView>("instructions");

            addButton = gameDocument.rootVisualElement.Q<Button>("add-button");
            deleteButton = gameDocument.rootVisualElement.Q<Button>("delete-button");
            undoButton = gameDocument.rootVisualElement.Q<Button>("undo-button");
            redoButton = gameDocument.rootVisualElement.Q<Button>("redo-button");
            copyButton = gameDocument.rootVisualElement.Q<Button>("copy-button");
            pasteButton = gameDocument.rootVisualElement.Q<Button>("paste-button");

            addButton.clicked += addInstructionMenu.StartAddingInstruction;
            deleteButton.clicked += DeleteSelected;
            undoButton.clicked += commandStateController.Undo;
            redoButton.clicked += commandStateController.Redo;
            copyButton.clicked += CopySelected;
            pasteButton.clicked += Paste;

            instructionsContainer.contentContainer.RegisterCallback<PointerMoveEvent>((e) => OnMouseOverList(e));

            instructionsContainer.RegisterCallback<NavigationMoveEvent>((e) => OnNavigationMove(e));
            instructionsContainer.RegisterCallback<FocusEvent>((e) => OnWindowFocused(e), TrickleDown.TrickleDown);
            instructionsContainer.RegisterCallback<FocusInEvent>((e) => OnBarFocused(e));

            gameDocument.rootVisualElement.RegisterCallback<NavigationMoveEvent>((e) => OnGlobalNavigationMove(e), TrickleDown.TrickleDown);
            gameDocument.rootVisualElement.RegisterCallback<PointerCaptureOutEvent>((e) => OnPointerReleasedWithGrabbedBar(e));

            addInstructionMenu.Initialize(this, gameDocument);

            instructionsContainer.horizontalScroller.focusable = false;
            instructionsContainer.verticalScroller.focusable = false;
        }

        private void InitializeInputs()
        {
            addInput.action.performed += ctx => addInstructionMenu.StartAddingInstruction();
            deleteInput.action.performed += ctx => DeleteSelected();
            undoInput.action.performed += ctx => commandStateController.Undo();
            redoInput.action.performed += ctx => commandStateController.Redo();
            copyInput.action.performed += ctx => CopySelected();
            pasteInput.action.performed += ctx => Paste();
            replaceInput.action.performed += ctx => TryStartReplacingSelectedInstruction();



            selectAllInput.action.performed += ctx => SetAllBarsSelected(true);
        }

        private void Update()
        {
            UpdateEditorButtonsState();

            foreach (var instructionBar in instructionBars)
            {
                instructionBar.UpdateProgressBar();
            }

            instructionEditorContainer.EnableInClassList("no-instructions", instructionBars.Count == 0);
        }

        private void LateUpdate()
        {
            UpdateListPositioning();
        }

        private void UpdateListPositioning()
        {
            if (remainingRepositioningRequests == 0) return;
            remainingRepositioningRequests--;

            foreach (var instructionBar in instructionBars)
            {
                instructionBar.UpdateAbsolutePosition();
            }

            var desiredHeight = 0.0f;
            if(instructionBars.Count > 0)
            {
                var lastElement = instructionsContainer.ElementAt(instructionsContainer.childCount - 1);
                var lastElementStyle = lastElement.resolvedStyle;
                var containerStyle = instructionsContainer.contentContainer.resolvedStyle;
                desiredHeight = lastElement.style.top.value.value + lastElementStyle.marginTop + lastElementStyle.marginBottom + lastElementStyle.height;
                desiredHeight += containerStyle.paddingTop + containerStyle.paddingBottom;
            }
            instructionsContainer.contentContainer.style.height = desiredHeight;
        }

        private void OnMouseOverList(PointerMoveEvent evt)
        {
            mouseListRealPosition = evt.localPosition.y;

            UpdateMouseListBasedPosition();
            HandleBarGrabbing();
        }

        private void UpdateMouseListBasedPosition()
        {
            // calculate list-based mouse position (aka. over which element vertically mouse currently hovers)
            var yPosition = Mathf.Max(0.0f, mouseListRealPosition - instructionsContainer.contentContainer.resolvedStyle.paddingTop);

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


        private void HandleBarGrabbing()
        {
            if (addInstructionMenu.Opened || playingInstructions)
            {
                return;
            }

            if (!grabbing && currentlyHeldBar != null)
            {
                if (!currentlyHeldBar.IsPointerHoveringOnHandle())
                {
                    var selectedBars = instructionBars.Where(bar => bar.Selected);

                    if (selectedBars.Any())
                    {
                        grabbing = true;
                        commandStateController.StoreUndoOperation(
                            Mathf.RoundToInt((float)selectedBars.Select(bar => instructionBars.IndexOf(bar)).Average()),
                            InstructionsStateController.StateType.MovedDrag
                        );
                    }
                }
            }

            if (!grabbing) return;

            int newPos = Mathf.FloorToInt(mouseListBasedPosition);
            int oldPos = instructionBars.IndexOf(lastFocusedInstructionBar);

            int delta = newPos - oldPos;
            if(delta != 0)
            {
                MoveSelectedInstructions(delta, true);
            }

            // scroll list up or down if near the edge
            var mouseScreenPosition = mouseListRealPosition - instructionsContainer.scrollOffset.y;
            var halfListHeight = instructionsContainer.localBound.height * 0.5f;
            var midDistance = mouseScreenPosition - halfListHeight;
            var edgeSpeed = Math.Clamp((Math.Abs(midDistance) - (halfListHeight - focusZoneEdgesSize)), 0.0f, 1.0f);
            edgeSpeed *= Math.Sign(midDistance) / focusZoneEdgesSize;

            if(edgeSpeed != 0.0f)
            {
                instructionsContainer.scrollOffset += Vector2.up * edgeSpeed * grabScrollMaxSpeed;
            }
        }

        private void MoveSelectedInstructions(int direction, bool dragged)
        {
            var newPos = instructionBars.IndexOf(lastFocusedInstructionBar) + direction;
            var selectedBars = instructionBars.Where(bar => bar.Selected).ToList();

            int minPos = newPos - selectedBars.IndexOf(lastFocusedInstructionBar);
            int maxPos = minPos + selectedBars.Count - 1;

            if(minPos < 0 || maxPos >= instructionsContainer.childCount)
            {
                return;
            }

            if (!dragged)
            {
                commandStateController.StoreUndoOperation(
                    Mathf.RoundToInt((float)selectedBars.Select(bar => instructionBars.IndexOf(bar)).Average()),
                    InstructionsStateController.StateType.MovedShift
                );
            }

            // temporarily "remove" grabbed instructions so they can be moved
            foreach (var bar in selectedBars)
            {
                bar.BringToFront();
                instructionBars.Remove(bar);
            }

            for (int i = 0; i < selectedBars.Count; i++)
            {
                var bar = selectedBars[i];
                instructionsContainer.Insert(minPos + i, bar);
                instructionBars.Insert(minPos + i, bar);
            }

            remainingRepositioningRequests++;

            if (!dragged)
            {
                InstructionsListModified();
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
                undoButton.SetEnabled(commandStateController.GetCurrentSlot().Undos.Count > 0);
                redoButton.SetEnabled(commandStateController.GetCurrentSlot().Redos.Count > 0);
                copyButton.SetEnabled(deleteButton.enabledSelf);
                pasteButton.SetEnabled(RobotInstruction.IsValidListString(GUIUtility.systemCopyBuffer));
            }
        }

        private void OnGlobalNavigationMove(NavigationMoveEvent evt)
        {
            if (instructionsContainer.ContainsElement(evt.target as VisualElement))
            {
                return;
            }

            if (evt.direction != NavigationMoveEvent.Direction.Up && evt.direction != NavigationMoveEvent.Direction.Down)
            {
                return;
            }

            evt.PreventDefault();
            doNotUnselectNextFocus = true;
            if (lastFocusedInstructionBar != null)
            {
                lastFocusedInstructionBar.Focus();
            }
            else if(instructionBars.Count > 0){
                instructionBars[(evt.direction == NavigationMoveEvent.Direction.Up) ? ^1 : 0].Focus();
            }
        }

        private void OnNavigationMove(NavigationMoveEvent evt)
        {
            grabbing = false;

            if (lastFocusedInstructionBar == null)
            {
                evt.PreventDefault();

                if(instructionBars.Count > 0)
                {
                    instructionBars[(evt.direction == NavigationMoveEvent.Direction.Up) ? ^1 : 0].Focus();
                }

                return;
            }

            var leaveDirection = evt.direction switch
            {
                NavigationMoveEvent.Direction.Previous => NavigationMoveEvent.Direction.Previous,
                NavigationMoveEvent.Direction.Next => NavigationMoveEvent.Direction.Down,
                _ => NavigationMoveEvent.Direction.None
            };
            if(leaveDirection != NavigationMoveEvent.Direction.None)
            {
                evt.PreventDefault();
                var nextFocus = instructionsContainer.focusController.GetNextFocusable(instructionsContainer, leaveDirection);
                nextFocus.Focus();
                return;
            }


            var focusedBarIndex = instructionBars.IndexOf(lastFocusedInstructionBar);

            var nextFocusIndex = focusedBarIndex + evt.direction switch
            {
                NavigationMoveEvent.Direction.Up => -1,
                NavigationMoveEvent.Direction.Down => 1,
                _ => 0
            };

            if (nextFocusIndex < 0 || nextFocusIndex >= instructionBars.Count)
            {
                evt.PreventDefault();
                return;
            }

            if (rangeSelectionInput.action.IsPressed())
            {
                evt.PreventDefault();
                MoveSelectedInstructions(nextFocusIndex - focusedBarIndex, false);
                return;
            }

            if (!multiSelectionInput.action.IsPressed())
            {
                SetAllBarsSelected(false);
            }

            evt.PreventDefault();
            instructionBars[nextFocusIndex].Focus();
        }

        private void OnWindowFocused(FocusEvent evt)
        {
            // special case for tabbing to previous element
            if (evt.direction.GetValue() == 1 && lastFocusedInstructionBar != null)
            {
                doNotUnselectNextFocus = true;
                lastFocusedInstructionBar.Focus();
                return;
            }

            if (evt.target != instructionsContainer) return;

            if (instructionBars.Count() == 0) return;

            if (evt.relatedTarget == null || evt.relatedTarget is InstructionBar)
            {
                if(!multiSelectionInput.action.IsPressed() && !rangeSelectionInput.action.IsPressed())
                {
                    SetAllBarsSelected(false);
                    lastFocusedInstructionBar = null;
                }
            }

            else if(evt.relatedTarget != null)
            {
                var navDir = evt.direction.GetValue();
                var tabNavigation = navDir == 1 || navDir == 2 || navDir == 5 || navDir == 6;
                if (lastFocusedInstructionBar != null && tabNavigation)
                {
                    doNotUnselectNextFocus = true;
                    lastFocusedInstructionBar.Focus();
                }
                else
                {
                    lastFocusedInstructionBar = null;
                    bool useLast = (evt.relatedTarget as VisualElement).worldBound.y > instructionsContainer.worldBound.y;
                    var barToFocus = instructionBars[useLast ? ^1 : 0];
                    barToFocus.Focus();
                }
            }
            
        }

        private void OnBarFocused(FocusInEvent evt)
        {
            var focusedBarQuery = instructionBars.Where(bar => bar.ContainsElement(evt.target as VisualElement));
            if (!focusedBarQuery.Any()) return;
            
            var bar = focusedBarQuery.First();

            foreach (var barToBlur in instructionBars)
            {
                if (bar != barToBlur) barToBlur.Blur();
            }

            if (
                !multiSelectionInput.action.IsPressed() && 
                !rangeSelectionInput.action.IsPressed() &&
                !bar.Selected &&
                !doNotUnselectNextFocus
            )
            {
                SetAllBarsSelected(false);
            }
            doNotUnselectNextFocus = false;

            if (rangeSelectionInput.action.IsPressed())
            {
                var lastBarIndex = instructionBars.IndexOf(lastFocusedInstructionBar);
                var clickedBarIndex = instructionBars.IndexOf(bar);

                for(var i = Mathf.Min(lastBarIndex, clickedBarIndex) + 1; i <= Mathf.Max(lastBarIndex, clickedBarIndex) - 1; i++)
                {
                    instructionBars[i].Selected = true;
                }
            }

            bar.Selected = true;
            ScrollToInstruction(instructionBars.IndexOf(bar), false);
            lastFocusedInstructionBar = bar;
        }

        private void OnBarPointerDown(PointerDownEvent e, InstructionBar bar)
        {
            currentlyHeldBar = bar;
            instructionsContainer.contentContainer.CapturePointer(e.pointerId);

            var delay = Time.time - lastClickedInstructionBarTime;
            if (bar == lastFocusedInstructionBar && delay < doubleClickMaxDelay)
            {
                TryStartReplacingSelectedInstruction();
            }
            lastClickedInstructionBarTime = Time.time;
        }

        private void OnPointerReleasedWithGrabbedBar(PointerCaptureOutEvent evt)
        {
            instructionsContainer.contentContainer.ReleasePointer(evt.pointerId);

            if (grabbing)
            {
                grabbing = false;
                InstructionsListModified();
            }
            else if (
                currentlyHeldBar != null && 
                !multiSelectionInput.action.IsPressed() && 
                !rangeSelectionInput.action.IsPressed()
            )
            {
                SetAllBarsSelected(false);
                currentlyHeldBar.Selected = true;
            }

            currentlyHeldBar = null;
        }

        public InstructionBar CreateInstructionBarAt(RobotInstruction instruction, int index)
        {
            if (instruction == null)
            {
                return null;
            }

            var bar = new InstructionBar();

            bar.Selected = true;
            bar.Instruction = instruction.Clone() as RobotInstruction;
            bar.changing += () => commandStateController.StoreUndoOperation(index, InstructionsStateController.StateType.ParameterEdited);
            bar.changed += () => InstructionsListModified();
            bar.RegisterCallback<PointerDownEvent>(e => OnBarPointerDown(e, bar), TrickleDown.NoTrickleDown);

            instructionsContainer.Insert(index, bar);
            instructionBars.Insert(index, bar);

            return bar;
        }

        public void DeleteInstructionBar(InstructionBar bar)
        {
            instructionsContainer.Remove(bar);
            instructionBars.Remove(bar);
        }

        private void SetAllBarsSelected(bool selected)
        {
            if (!instructionsContainer.enabledSelf) return;

            foreach(var bar in instructionBars)
            {
                bar.Selected = selected;
            }
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
                commandStateController.StoreUndoOperation(
                    addIndex + instructions.Count / 2,
                    InstructionsStateController.StateType.Added
                );
            }

            foreach (var selectedBar in selected)
            {
                selectedBar.Selected = false;
            }

            var initialAddIndex = addIndex;

            foreach (RobotInstruction instruction in instructions)
            {
                var bar = CreateInstructionBarAt(instruction, addIndex + 1);
                if (!quiet)
                {
                    bar.SetInterpolationStartingPosition(initialAddIndex);
                }
                addIndex++;
            }

            InstructionsListModified();
        }

        public void AddInstruction(RobotInstruction instruction, bool quiet = false)
        {
            AddInstructions(new List<RobotInstruction>() { instruction }, quiet);
        }

        public void TryStartReplacingSelectedInstruction()
        {
            if (!instructionsContainer.enabledSelf || addInstructionMenu.Opened) return;

            var selectedBars = instructionBars.Where(i => i.Selected);
            if (selectedBars.Count() == 1)
            {
                var bar = selectedBars.First();
                var barIndex = instructionBars.IndexOf(bar);
                addInstructionMenu.StartReplacingInstruction(barIndex, bar.Instruction.Action);
            }
        }

        public void ReplaceInstruction(int index, RobotInstruction instruction)
        {
            if (index < 0 || index >= instructionBars.Count) return;

            commandStateController.StoreUndoOperation(index, InstructionsStateController.StateType.TypeChanged);

            instructionBars.RemoveAt(index);
            instructionsContainer.RemoveAt(index);

            CreateInstructionBarAt(instruction, index);

            InstructionsListModified();
        }

        public void InstructionsListModified()
        {
            // instructions added need to be repositioned multiple times
            // due to having different starting position
            remainingRepositioningRequests += 2;

            var levelInfo = SaveManager.CurrentSave.GetCurrentLevelInfo();

            levelInfo.Slots[currentSlot].Instructions = GetRobotInstructionsList();

            grabbing = false;
        }

        public void LoadSaveSlot(int slot)
        {
            foreach (var bar in instructionBars.ToList())
            {
                DeleteInstructionBar(bar);
            }

            currentSlot = slot;

            var saveLevelData = SaveManager.CurrentSave.GetCurrentLevelInfo();
            if (saveLevelData != null)
            {
                AddInstructions(saveLevelData.Slots[currentSlot].Instructions, true);
                SetAllBarsSelected(false);
            }
        }

        public void DeleteSelected()
        {
            if (!instructionsContainer.enabledSelf) return;

            var selectedBars = instructionBars.Where(bar => bar.Selected).ToList();

            if (selectedBars.Any())
            {
                var barToSelectAfterwards = Math.Max(0, selectedBars.Select(bar => instructionBars.IndexOf(bar)).Min());

                var focusIndex = selectedBars.Select(bar => instructionBars.IndexOf(bar)).Average();
                commandStateController.StoreUndoOperation(
                    Mathf.RoundToInt((float)focusIndex),
                    InstructionsStateController.StateType.Deleted
                );

                foreach (var bar in selectedBars)
                {
                    DeleteInstructionBar(bar);
                }

                if (barToSelectAfterwards >= 0 && instructionBars.Count > 0)
                {
                    barToSelectAfterwards = Math.Min(barToSelectAfterwards, instructionBars.Count - 1);
                    instructionBars[barToSelectAfterwards].Selected = true;
                    instructionBars[barToSelectAfterwards].Focus();
                }

                InstructionsListModified();
            }
        }

        public void CopySelected()
        {
            if (!instructionsContainer.enabledSelf)
            {
                return;
            }
            if (instructionEditorContainer.panel.focusController.focusedElement is TextField)
            {
                return;
            }

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
            if (instructionEditorContainer.panel.focusController.focusedElement is TextField)
            {
                return;
            }
            if (!instructionsContainer.enabledSelf) return;

            var clipboard = GUIUtility.systemCopyBuffer;
            if (RobotInstruction.IsValidListString(clipboard))
            {
                AddInstructions(RobotInstruction.StringToList(clipboard));
            }
        }

        public void ScrollToInstruction(int instructionIndex, bool center)
        {
            if (instructionBars.Count == 0) return;
            var bar = instructionBars[Math.Clamp(instructionIndex, 0, instructionBars.Count - 1)];

            // Bars that have been created this frame might not have been
            // fully recalculated yet. Delay setting scroll offset until
            // the UI Toolkit is done with recalculations, so it isn't just set to 0.
            StartCoroutine(ScrollToInstructionBarCoroutine(bar, center));
        }

        private IEnumerator ScrollToInstructionBarCoroutine(InstructionBar bar, bool center)
        {
            // wait one frame. awful, but what can I do?!
            yield return null;

            var barPos = bar.localBound.y;
            var barHeight = bar.localBound.height;
            var height = instructionsContainer.localBound.height;
            var targetYScroll = instructionsContainer.scrollOffset.y;

            if (center)
            {
                // make instruction appear in the middle of the scrolling container
                targetYScroll = barPos - height * 0.5f;
            }
            else
            {
                var barIndex = instructionBars.IndexOf(bar);
                // make instruction appear within the scrolling container bounds
                if(barPos < instructionsContainer.scrollOffset.y)
                {
                    targetYScroll = barPos;
                    if (barIndex == 0) targetYScroll = 0;
                }
                else if(barPos + barHeight > instructionsContainer.scrollOffset.y + height)
                {
                    targetYScroll = (barPos - height + barHeight);
                    if (barIndex == instructionBars.Count - 1) targetYScroll += barHeight;
                }
            }
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
                ScrollToInstruction(instructionIndex, true);
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
