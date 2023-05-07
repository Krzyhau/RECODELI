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
        [SerializeField] private RectTransform validClickArea;
        [SerializeField] private GameObject noInstructionsPopup;
        [SerializeField] private GameObject instructionsMenu;
        [SerializeField] private GameObject addInstructionMenu;
        [SerializeField] private float barsPadding;
        [SerializeField] private float barsPositionInterpolation;

        [Header("Buttons")]
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button undoButton;
        [SerializeField] private Button redoButton;

        private Canvas canvasContainer;

        private List<InstructionBar> grabbedInstructions = new List<InstructionBar>();
        private bool grabbing;
        private bool clickedInClickZone;
        private bool addInstructionMenuActive;
        private bool addInstructionMenuWasActive;
        private InstructionBar lastClickedInstructionBar;
        private float buttonBasedMousePosition;

        public bool Grabbing => grabbing;

        private void OnEnable()
        {
            deleteButton.onClick.AddListener(DeleteSelected);
            undoButton.onClick.AddListener(Undo);
            redoButton.onClick.AddListener(Redo);
        }

        private void OnDisable()
        {
            deleteButton.onClick.RemoveListener(DeleteSelected);
            undoButton.onClick.RemoveListener(Undo);
            redoButton.onClick.RemoveListener(Redo);
        }

        private void Awake()
        {
            // this is needed to prevent comma from being used as decimal indicator in some countries.
            // TODO: move it somewhere else once a fitting place is made
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            canvasContainer = instructionsContainer.GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            ValidateBarsChildren();
            RecalculateButtonBasedMousePosition();
            UpdateBarsPosition();

            if (!addInstructionMenuWasActive)
            {
                UpdateBarsSelection();
                HandleBarGrabbing();
            }
            else if (!addInstructionMenuActive)
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
            if (Input.GetMouseButtonUp(0) && !grabbing && !rangeSelection && !multipleSelection)
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
            if (addInstructionMenuActive)
            {
                deleteButton.interactable = false;
                undoButton.interactable = false;
                redoButton.interactable = false;
            }
            else
            {
                deleteButton.interactable = EnumerateBars().Where(bar => bar.Selected).Any();
                undoButton.interactable = false;
                redoButton.interactable = false;
            }
        }

        private void HandleKeyboardShortcuts()
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    foreach (var bar in EnumerateBars()) bar.Selected = true;
                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    // TODO: implement copy here
                }
                else if (Input.GetKeyDown(KeyCode.V))
                {
                    // TODO: implement paste here
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    Undo();
                }
                else if (Input.GetKeyDown(KeyCode.Y))
                {
                    Redo();
                }
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


        public void DeleteSelected()
        {
            foreach (var bar in EnumerateBars().Where(bar => bar.Selected))
            {
                Destroy(bar.gameObject);
            }

            // TODO: add undo action here
        }

        public void Undo()
        {

        }

        public void Redo()
        {

        }

        public void SetAddInstructionMenuActive(bool active)
        {
            instructionsMenu.SetActive(!active);
            addInstructionMenu.SetActive(active);

            addInstructionMenuActive = active;
            if (active) addInstructionMenuWasActive = true;
        }

        public void AddInstruction(RobotInstruction instruction) => AddInstructions(new List<RobotInstruction>(){ instruction });
        public void AddInstructions(List<RobotInstruction> instructions)
        {
            foreach(RobotInstruction instruction in instructions)
            {
                var addIndex = instructionsContainer.childCount - 1;
                var selected = EnumerateBars().Where(bar => bar.Selected);
                if (selected.Any())
                {
                    addIndex = selected.Select(bar => bar.transform.GetSiblingIndex()).Max();
                    foreach (var selectedBar in selected)
                    {
                        selectedBar.Selected = false;
                    }
                }

                var yPos = addIndex < 0 ? 0.0f : (GetBar(addIndex).GetComponent<RectTransform>().anchoredPosition.y);

                var bar = Instantiate(instructionBarPrefab, instructionsContainer);

                bar.transform.SetSiblingIndex(addIndex + 1);
                bar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yPos);

                bar.Selected = true;
                bar.Instruction = instruction;

                SetAddInstructionMenuActive(false);
            }

            // TODO: Add undo action here
        }

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
            //throw new System.NotImplementedException();
        }

        public void SetPlaybackState(bool state)
        {
            //throw new System.NotImplementedException();
        }

        public List<RobotInstruction> GetRobotInstructionsList()
        {
            return EnumerateBars().Select(bar => bar.Instruction).ToList();
        }
    }
}
