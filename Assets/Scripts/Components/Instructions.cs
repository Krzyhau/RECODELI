using UnityEngine;
using Zenject;

namespace LudumDare.Scripts.Components
{
    public class Instructions : MonoBehaviour
    {
        private const float buttonOffset = 35f;

        [Inject] private InstructionButton instructionButtonPrefab;
        public Transform parent;
        public Transform addNewButton;

        public void AddInstruction(string text)
        {
            var newHeight = parent.GetChild(parent.childCount - 1).transform.position.y - buttonOffset;
            var newPrefab = Instantiate(instructionButtonPrefab, parent);
            newPrefab.ChangeLabel(text);
            newPrefab.transform.position = new Vector3(newPrefab.transform.position.x, newHeight, newPrefab.transform.position.z);
            RefreshAddNewButton();
        }

        private void RefreshAddNewButton()
        {

            addNewButton.transform.position = new Vector3(addNewButton.transform.position.x, addNewButton.transform.position.y - 35, addNewButton.transform.position.z);
        }
        public void RemoveInstruction()
        {

        }
    }
}
