using UnityEngine;


[System.Serializable]
public class DialogueLine
{
    public SpeakerData speaker;        
    [TextArea(2, 10)]
    public string dialogueText;      
   
    public DialogueChoice[] choices;
    public bool isNextScene = false;
}


[CreateAssetMenu(fileName ="NewDialogue", menuName ="Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
    public GameFlagKey[] requiredFlags;
    public GameFlagKey[] blockedIfFlags;
    public bool hideInactiveSpeaker = false;
    public int priority = 0;
}

