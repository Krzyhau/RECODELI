using LudumDare.Scripts.Models;
using LudumDare.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LudumDare.Scripts.Components.UI
{
    public class InstructionEditor : MonoBehaviour
    {
        [SerializeField] private RectTransform instructionsContainer;
        [SerializeField] private InstructionBar instructionBarPrefab;
        [SerializeField] private CanvasGroup validClickArea;
        [SerializeField] private InstructionsScrolling scrollingHandler;
        [SerializeField] private GameObject noInstructionsPopup;
        [SerializeField] private GameObject instructionsMenu;
        [SerializeField] private GameObject addInstructionMenu;
        [SerializeField] private float barsPadding;
        [SerializeField] private float barsPositionInterpolation;
        [SerializeField] private int maxUndoHistory;

        [Header("Buttons")]
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button undoButton;
        [SerializeField] private Button redoButton;
        [SerializeField] private Button copyButton;
        [SerializeField] private Button pasteButton;

        private struct RobotInstructionListCopy
        {
            public List<RobotInstruction> Instructions;
            public int ButtonBasedFocusPosition;
            public List<int> SelectionIndices;
        }

        private List<RobotInstructionListCopy> undoList;
        private List<RobotInstructionListCopy> redoList;

        private List<InstructionBar> grabbedInstructions = new List<InstructionBar>();
        private bool grabbing;
        private bool clickedInClickZone;
        private bool addInstructionMenuActive;
        private bool addInstructionMenuWasActive;
        private InstructionBar lastClickedInstructionBar;
        private float buttonBasedMousePosition;
        private bool playingInstructions;
        private Canvas canvasContainer;
        private float prePlayScrollPosition;

        public bool Grabbing => grabbing;

        private void OnEnable()
        {
            deleteButton.onClick.AddListener(DeleteSelected);
            undoButton.onClick.AddListener(Undo);
            redoButton.onClick.AddListener(Redo);
            copyButton.onClick.AddListener(CopySelected);
            pasteButton.onClick.AddListener(Paste);
        }

        private void OnDisable()
        {
            deleteButton.onClick.RemoveListener(DeleteSelected);
            undoButton.onClick.RemoveListener(Undo);
            redoButton.onClick.RemoveListener(Redo);
            copyButton.onClick.RemoveListener(CopySelected);
            pasteButton.onClick.RemoveListener(Paste);
        }

        private void Awake()
        {
            // this is needed to prevent comma from being used as decimal indicator in some countries.
            // TODO: move it somewhere else once a fitting place is made
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            canvasContainer = instructionsContainer.GetComponentInParent<Canvas>();

            undoList = new List<RobotInstructionListCopy>();
            redoList = new List<RobotInstructionListCopy>();
        }

        private void Update()
        {
            ValidateBarsChildren();
            RecalculateButtonBasedMousePosition();
            UpdateBarsPosition();

            if (!addInstructionMenuWasActive && !playingInstructions)
            {
                UpdateBarsSelection();
                HandleBarGrabbing();
            }
            if (addInstructionMenuWasActive && !addInstructionMenuActive)
            {
                addInstructionMenuWasActive = false;
            }

            UpdateButtons();

            HandleKeyboardShortcuts();

            var barCount = EnumerateBars().Count();
            if (barCount > 0 && noInstructionsPopup.activeSelf)
            {
                noInstructionsPopup.SetActive(false);
            }
            if (barCount == 0 && !noInstructionsPopup.activeSelf)
            {
                noInstructionsPopup.SetActive(true);
            }
        }

        private void ValidateBarsChildren()
        {
            foreach (Transform child in instructionsContainer)
            {
                if (!child.gameObject.TryGetComponent(out InstructionBar bar))
                {
                    Debug.LogError($"No InstructionBar component found in {child.gameObject}. Moving it out of the instructions container.");
                    child.SetParent(instructionsContainer.parent);
                }
            }
        }
        private void RecalculateButtonBasedMousePosition()
        {
            var mouseInRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                instructionsContainer, Input.mousePosition, canvasContainer.worldCamera, out var mousePosInRect
            );
            if (!mouseInRect) return;

            var yPosition = -mousePosInRect.y;

            if (yPosition < 0)
            {
                buttonBasedMousePosition = 0;
                return;
            }

            float currentPosition = -barsPadding * 0.5f;
            float buttonIndex = 0;
            foreach (RectTransform child in instructionsContainer)
            {
                var height = barsPadding + child.sizeDelta.y;

                if (yPosition >= currentPosition && yPosition < currentPosition + height)
                {
                    float innerFactor = (yPosition - currentPosition) / height;
                    buttonBasedMousePosition = buttonIndex + innerFactor;
                    return;
                }

                currentPosition += height;
                buttonIndex++;
            }

            buttonBasedMousePosition = buttonIndex;
        }

        private void UpdateBarsPosition()
        {
            float currentPosition = 0;
            foreach (RectTransform child in instructionsContainer)
            {
                var newPosition = Vector2.down * currentPosition;
                child.anchoredPosition = Vector2.Lerp(child.anchoredPosition, newPosition, barsPositionInterpolation * Time.unscaledDeltaTime);
                currentPosition += child.sizeDelta.y + barsPadding;
            }

            instructionsContainer.sizeDelta = new Vector2(0, currentPosition);
        }

        private void UpdateBarsSelection()
        {
            bool multipleSelection = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool rangeSelection = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            bool overClickZone = EventSystem.current.IsPointerOverGameObject(validClickArea.gameObject);

            if (Input.GetMouseButtonDown(0) && overClickZone)
            {
                InstructionBar clickedBar = EnumerateBars()
                    .Where(bar => bar.IsPointerHoveringOnHandle())
                    .FirstOrDefault();

                // unselect all when needed
                if ((clickedBar == null || !clickedBar.Selected) && !multipleSelection && !rangeSelection)
                {
                    foreach (InstructionBar bar in EnumerateBars())
                    {
                        bar.Selected = false;
                    }
                }

                // find out what to select
                if (clickedBar != null)
                {
                    if (lastClickedInstructionBar == null || !lastClickedInstructionBar.Selected)
                    {
                        lastClickedInstructionBar = EnumerateBars().Where(bar => bar.Selected).FirstOrDefault();
                    }

                    if (rangeSelection && lastClickedInstructionBar != null)
                    {
                        int startPos = lastClickedInstructionBar.transform.GetSiblingIndex();
                        int endPos = clickedBar.transform.GetSiblingIndex();
                        int step = startPos > endPos ? -1 : 1;
                        for (int i = startPos; i != endPos; i += step)
                        {
                            GetBar(i).Selected = true;
                        }
                    }

                    if (multipleSelection || !clickedBar.Selected)
                    {
                        clickedBar.Selected = !clickedBar.Selected;
                    }

                    lastClickedInstructionBar = clickedBar;
                }
            }

            // unselect other buttons
            if (Input.GetMouseButtonUp(0) && !grabbing && !rangeSelection && !multipleSelection && overClickZone)
            {
                foreach (InstructionBar bar in EnumerateBars())
                {
                    if (bar != lastClickedInstructionBar) bar.Selected = false;
                }
            }

        }

        private void HandleBarGrabbing()
        {
            bool overClickZone = EventSystem.current.IsPointerOverGameObject(validClickArea.gameObject);

            if (Input.GetMouseButtonDown(0))
            {
                clickedInClickZone = overClickZone;
            }

            if (Input.GetMouseButton(0) && lastClickedInstructionBar != null && !grabbing && clickedInClickZone)
            {
                if (!lastClickedInstructionBar.IsPointerHoveringOnHandle())
                {
                    var selectedBars = EnumerateBars().Where(bar => bar.Selected);

                    if (selectedBars.Any())
                    {
                        grabbing = true;
                        grabbedInstructions = selectedBars.ToList();
                        StoreUndoOperation(Mathf.RoundToInt((float)selectedBars.Select(bar => bar.transform.GetSiblingIndex()).Average()));
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && grabbing)
            {
                grabbing = false;
                grabbedInstructions.Clear();
            }

            if (grabbing)
            {
                int newPos = Mathf.FloorToInt(buttonBasedMousePosition);
                int oldPos = lastClickedInstructionBar.transform.GetSiblingIndex();

                int minPos = newPos - grabbedInstructions.IndexOf(lastClickedInstructionBar);
                int maxPos = minPos + grabbedInstructions.Count - 1;

                if (newPos != oldPos && minPos >= 0 && maxPos < instructionsContainer.childCount)
                {
                    // move all grabbed bars to the very end first so they don't get mixed up later
                    foreach (var bar in grabbedInstructions)
                    {
                        bar.transform.SetSiblingIndex(instructionsContainer.childCount - 1);
                    }

                    int currPos = minPos;

                    foreach (var bar in grabbedInstructions)
                    {
                        bar.transform.SetSiblingIndex(currPos);
                        currPos++;
                    }
                }
            }
        }

        private void UpdateButtons()
        {
            if (addInstructionMenuActive || playingInstructions)
            {
                deleteButton.interactable = false;
                undoButton.interactable = false;
                redoButton.interactable = false;
                copyButton.interactable = false;
                pasteButton.interactable = false;
            }
            else
            {
                deleteButton.interactable = EnumerateBars().Where(bar => bar.Selected).Any();
                undoButton.interactable = undoList.Count > 0;
                redoButton.interactable = redoList.Count > 0;
                copyButton.interactable = deleteButton.interactable;
                pasteButton.interactable = RobotInstruction.IsValidListString(GUIUtility.systemCopyBuffer);
            }
        }

        private void HandleKeyboardShortcuts()
        {
            if (!addInstructionMenuActive && !playingInstructions)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    foreach (var bar in EnumerateBars()) bar.Selected = true;
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
                    foreach (var bar in EnumerateBars()) bar.Selected = false;
                }
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    DeleteSelected();
                }
            }
            else if(addInstructionMenuActive)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SetAddInstructionMenuActive(false);
                }
            }
        }


        private InstructionBar CreateInstructionBarAt(RobotInstruction instruction, int index)
        {
            var bar = Instantiate(instructionBarPrefab, instructionsContainer);

            bar.transform.SetSiblingIndex(index);
            float buttonPosition = (instructionBarPrefab.GetComponent<RectTransform>().sizeDelta.y + barsPadding) * index;
            bar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -buttonPosition);

            bar.Selected = true;
            bar.Instruction = (RobotInstruction)instruction.Clone();

            return bar;
        }

        private void StoreUndoOperation(int focusPosition)
        {
            undoList.Add(new RobotInstructionListCopy
            {
                Instructions = EnumerateBars().Select(bar => bar.Instruction).ToList(),
                ButtonBasedFocusPosition = focusPosition,
                SelectionIndices = EnumerateBars().Where(bar => bar.Selected).Select(bar => bar.transform.GetSiblingIndex()).ToList()
            });

            if(undoList.Count > maxUndoHistory)
            {
                undoList.RemoveAt(0);
            }

            redoList.Clear();
        }

        private void AddInstructions(List<RobotInstruction> instructions)
        {
            var addIndex = instructionsContainer.childCount - 1;
            var selected = EnumerateBars().Where(bar => bar.Selected);
            if (selected.Any())
            {
                addIndex = selected.Select(bar => bar.transform.GetSiblingIndex()).Max();
            }

            StoreUndoOperation(addIndex + instructions.Count / 2);

            foreach (var selectedBar in selected)
            {
                selectedBar.Selected = false;
            }

            foreach (RobotInstruction instruction in instructions)
            {
                CreateInstructionBarAt(instruction, addIndex + 1);

                addIndex++;
            }
            SetAddInstructionMenuActive(false);
        }

        public void DeleteSelected()
        {
            var selectedBars = EnumerateBars().Where(bar => bar.Selected);

            if (selectedBars.Any())
            {
                var barToSelectAfterwards = selectedBars.Select(bar => bar.transform.GetSiblingIndex()).Min() - 1;
                if(barToSelectAfterwards < 0)
                {
                    var nonSelectedBars = EnumerateBars().Where(bar => !bar.Selected);
                    if (nonSelectedBars.Any())
                    {
                        barToSelectAfterwards = nonSelectedBars.Select(bar => bar.transform.GetSiblingIndex()).Min();
                    }
                }

                var focusIndex = selectedBars.Select(bar => bar.transform.GetSiblingIndex()).Average();
                StoreUndoOperation(Mathf.RoundToInt((float)focusIndex));

                foreach (var bar in selectedBars)
                {
                    Destroy(bar.gameObject);
                }

                if (barToSelectAfterwards >= 0)
                {
                    GetBar(barToSelectAfterwards).Selected = true;
                }
            }
        }

        public void Undo()
        {
            if (undoList.Count == 0) return;
            var undo = undoList.Last();

            var focusPos = undo.ButtonBasedFocusPosition;

            redoList.Add(new RobotInstructionListCopy
            {
                Instructions = EnumerateBars().Select(bar => bar.Instruction).ToList(),
                ButtonBasedFocusPosition = focusPos,
                SelectionIndices = EnumerateBars().Where(bar => bar.Selected).Select(bar => bar.transform.GetSiblingIndex()).ToList()
            });

            foreach (var bar in EnumerateBars())
            {
                bar.gameObject.SetActive(false);
                Destroy(bar.gameObject);
            }

            var currentIndex = 0;
            foreach (var instruction in undo.Instructions)
            {
                var bar = CreateInstructionBarAt(instruction, currentIndex);
                bar.Selected = undo.SelectionIndices.Contains(currentIndex);
                currentIndex++;
            }

            undoList.RemoveAt(undoList.Count - 1);

            grabbing = false;

            float buttonFocusPosition = (instructionBarPrefab.GetComponent<RectTransform>().sizeDelta.y + barsPadding) * focusPos;
            scrollingHandler.InterpolateToPosition(buttonFocusPosition);
        }

        public void Redo()
        {
            if (redoList.Count == 0) return;
            var redo = redoList.Last();
            var focusPos = redo.ButtonBasedFocusPosition;

            undoList.Add(new RobotInstructionListCopy
            {
                Instructions = EnumerateBars().Select(bar => bar.Instruction).ToList(),
                ButtonBasedFocusPosition = focusPos,
                SelectionIndices = EnumerateBars().Where(bar => bar.Selected).Select(bar => bar.transform.GetSiblingIndex()).ToList()
            });

            foreach (var bar in EnumerateBars())
            {
                bar.gameObject.SetActive(false);
                Destroy(bar.gameObject);
            }

            var currentIndex = 0;
            
            foreach (var instruction in redo.Instructions)
            {
                var bar = CreateInstructionBarAt(instruction, currentIndex);
                bar.Selected = redo.SelectionIndices.Contains(currentIndex);
                currentIndex++;
            }

            redoList.RemoveAt(redoList.Count - 1);

            grabbing = false;

            float buttonFocusPosition = (instructionBarPrefab.GetComponent<RectTransform>().sizeDelta.y + barsPadding) * focusPos;
            scrollingHandler.InterpolateToPosition(buttonFocusPosition);
        }

        public void CopySelected()
        {
            var selectedBars = EnumerateBars().Where(bar => bar.Selected);

            if (selectedBars.Any())
            {
                GUIUtility.systemCopyBuffer = RobotInstruction.ListToString(
                    selectedBars.Select(bar=>bar.Instruction).ToList()
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

        public void SetAddInstructionMenuActive(bool active)
        {
            instructionsMenu.SetActive(!active);
            addInstructionMenu.SetActive(active);

            addInstructionMenuActive = active;
            if (active) addInstructionMenuWasActive = true;
        }

        public void AddInstruction(RobotInstruction instruction) => AddInstructions(new List<RobotInstruction>(){ instruction });

        public InstructionBar GetBar(int i)
        {
            Assert.IsTrue(i >= 0 && i < instructionsContainer.childCount, "Invalid instruction bar index requested.");
            return instructionsContainer.GetChild(i).GetComponent<InstructionBar>();
        }
        public IEnumerable<InstructionBar> EnumerateBars()
        {
            for (int i = 0; i < instructionsContainer.childCount; i++)
            {
                yield return GetBar(i);
            }
        }

        public void HighlightInstruction(int instructionIndex)
        {
            if (instructionIndex == -1)
            {
                scrollingHandler.InterpolateToPosition(prePlayScrollPosition, true);
            }
            else
            {
                float buttonFocusPosition = (instructionBarPrefab.GetComponent<RectTransform>().sizeDelta.y + barsPadding) * instructionIndex;
                scrollingHandler.InterpolateToPosition(buttonFocusPosition);
            }
        }

        public void SetPlaybackState(bool state)
        {
            playingInstructions = state;
            if (!state)
            {
                foreach(var bar in EnumerateBars())
                {
                    bar.Instruction.UpdateProgress(0.0f);
                }
            }

            validClickArea.interactable = !state;
            if (state)
            {
                prePlayScrollPosition = scrollingHandler.CurrentScrollPosition;
                foreach (var bar in EnumerateBars())
                {
                    bar.Selected = false;
                }
            }
        }

        public List<RobotInstruction> GetRobotInstructionsList()
        {
            return EnumerateBars().Select(bar => bar.Instruction).ToList();
        }
    }
}
