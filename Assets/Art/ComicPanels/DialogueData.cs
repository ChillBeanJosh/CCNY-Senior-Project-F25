using UnityEngine;

[System.Serializable]
public class DialogueSequence
{
    public string[] lines;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueSequence[] sequences;
    
}
