using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueLine
{
    public SpeakerData speaker;
    [TextArea(2, 10)]
    public string dialogueText;

    public DialogueChoice[] choices;
    public bool isNextScene = false;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
    public GameFlagKey[] requiredFlags;
    public GameFlagKey[] blockedIfFlags;
    public bool hideInactiveSpeaker = false;
    public int priority = 0;

    [Header("대사 종료 시 오브젝트 활성/비활성 처리")]
    public GameObject objectsToActivateOnEnd;      // 대사 끝나면 활성화할 오브젝트들
    public GameObject objectsToDeactivateOnEnd;    // 대사 끝나면 비활성화할 오브젝트들
}