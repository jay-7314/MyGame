using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


//대사 한줄짜리 하기 위한 class
[System.Serializable]
public class DialogueLine
{
    public SpeakerData speaker;         //말하는 사람

    [TextArea(2, 10)]
    public string dialogueText;         //말하는 공간
   
    public DialogueChoice[] choices;
    public bool isNextScene = false;
}


//대사 여러줄을 하기 위한 스크렙트오브젝트
[CreateAssetMenu(fileName ="NewDialogue", menuName ="Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
    public GameFlagKey[] requiredFlags;
    public GameFlagKey[] blockedIfFlags;
    public bool hideInactiveSpeaker = false;
    public int priority = 0;
}

