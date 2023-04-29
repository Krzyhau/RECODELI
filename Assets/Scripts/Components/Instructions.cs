using LudumDare.Scripts.Utils;
using UnityEngine;
using Zenject;

namespace LudumDare.Scripts.Components
{
    public class Instructions : MonoBehaviour
    {
        public const float buttonOffset = 35f;

        [Inject] private readonly InstructionButton instructionButtonPrefab;

        [SerializeField] private RectTransform instructionsListParent;
        [SerializeField] private RectTransform addNewButtonTransform;

        public void AddInstruction(string text)
        {
            float newHeight = instructionsListParent.GetChild(instructionsListParent.childCount - 1)
                .transform.position.y - buttonOffset;
            var newPrefab = Instantiate(instructionButtonPrefab, instructionsListParent);
            
            newPrefab.ChangeLabel(text);
            newPrefab.transform.position.ChangeY(newHeight);
            
            RefreshAddNewButton();
        }

        public void RemoveInstruction()
        {

        }

        private void RefreshAddNewButton()
        {
            addNewButtonTransform.transform.position.ChangeY
                (addNewButtonTransform.transform.position.y - buttonOffset);
        }
    }
}
