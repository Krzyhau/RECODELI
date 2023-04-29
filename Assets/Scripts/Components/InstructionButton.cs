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
    }
}
