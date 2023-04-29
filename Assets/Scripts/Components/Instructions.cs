using UnityEngine;
using Zenject;

namespace LudumDare.Scripts.Components
{
    public class Instructions : MonoBehaviour
    {
        private const float buttonOffset = 35f;

        [Inject] private InstructionButton instructionButtonPrefab;

        [SerializeField] private RectTransform instructionsListParent;
        [SerializeField] private RectTransform addNewButtonTransform;

        public void AddInstruction(string text)
        {
            var newHeight = instructionsListParent.GetChild(instructionsListParent.childCount - 1)
                .transform.position.y - buttonOffset;
            var newPrefab = Instantiate(instructionButtonPrefab, instructionsListParent);
            newPrefab.ChangeLabel(text);
            newPrefab.transform.position = new (newPrefab.transform.position.x, newHeight, newPrefab.transform.position.z);
            RefreshAddNewButton();
        }

        public void RemoveInstruction()
        {

        }

        private void RefreshAddNewButton()
        {
            addNewButtonTransform.transform.position = new(addNewButtonTransform.transform.position.x, addNewButtonTransform.transform.position.y - 35, addNewButtonTransform.transform.position.z);
        }
    }
}
