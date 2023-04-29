using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour
{
    public InstructionButton prefab;
    public Transform parent;
    public void AddInstruction(string text)
    {
        var newPrefab = Instantiate(prefab,parent) ;
        newPrefab.ChangeLabel(text);
    }
    public void RemoveInstruction()
    {

    }
}
