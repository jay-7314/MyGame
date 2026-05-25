using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField] GameObject talkUI;
    [SerializeField] Text speakerName;
    [SerializeField] Image speakerImg;
    [SerializeField] Text dialogue;

    DialogueData currentDialogueData;                           //현재 대사 데이터
    int currentIndex;                                           //현재 대사 인덱스
    Action onEndCallback;                                       //대사 종료시 호출할 콜백??

    public bool isDialogueCheck { get; private set; }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        talkUI.SetActive(false);
    }

    public void StartDialogue(DialogueData data, Action onEnd = null)
    {
        currentDialogueData = data;
        currentIndex = 0;
        onEndCallback = onEnd;
        isDialogueCheck = true;

        talkUI.SetActive(true);
        ShowCurrentLine();
    }
    
    public void NextLine()
    {
        if (!isDialogueCheck) return;

        currentIndex++;
        if(currentIndex >= currentDialogueData.lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        DialogueLine line = currentDialogueData.lines[currentIndex];

        if(line.speaker!= null)
        {
            speakerName.text = line.speaker.speakerName;
            speakerImg.sprite = line.speaker.speakerImg;
        }

        dialogue.text = line.dialogueText;
    }

    void EndDialogue()
    {
        isDialogueCheck = false;
        talkUI.SetActive(false);

        Action callback = onEndCallback;
        onEndCallback = null;
        callback?.Invoke();
    }

  


}
