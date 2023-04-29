using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionButton : MonoBehaviour
{
    public Text TextLabel;
    public Text TextInput;
    public void ChangeLabel(string label)
    {
        TextLabel.text = label;
    }

}
