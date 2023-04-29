using UnityEngine;
using UnityEngine.UI;

namespace LudumDare.Scripts.Components
{
    public class InstructionButton : MonoBehaviour
    {
        [SerializeField] private Text textLabel;
        [SerializeField] private Text textInput;

        public void ChangeLabel(string label)
        {
            textLabel.text = label;
        }
        public void ChangePosition()
        {

        }
        public void RemoveInstruction()
        {
            foreach(Transform child in transform.parent)
            {
                Debug.Log(child.name);
            }
            Destroy(gameObject);
        }
    }
}
