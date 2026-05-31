using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField] GameObject talkUI;
    [SerializeField] Text speakerName;
    [SerializeField] Text dialogue;

    [SerializeField] Image leftCharacterImg;
    [SerializeField] Image rightCharacterImg;

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
        if (line.speaker == null) return;

       speakerName.text = line.speaker.speakerName;
        bool isTwoSpeakers = HasMultiSpeaker();

        if (line.speaker.speakerType == SpeakerData.SpeakerType.Player)
        {
            leftCharacterImg.sprite = line.speaker.speakerImg;
            leftCharacterImg.gameObject.SetActive(true);
            rightCharacterImg.gameObject.SetActive(isTwoSpeakers);

            SetActive(leftCharacterImg);
            if(isTwoSpeakers) SetInactive(rightCharacterImg);
        }

        else
        {
            rightCharacterImg.sprite = line.speaker.speakerImg;
            rightCharacterImg.gameObject.SetActive(true);
            leftCharacterImg.gameObject.SetActive(isTwoSpeakers);

            SetActive(rightCharacterImg);
            if (isTwoSpeakers) SetInactive(leftCharacterImg);
        }
        dialogue.text = line.dialogueText;
    }

    bool HasMultiSpeaker()
    {
        bool hasPlayer = false;
        bool hasNPC = false;

        for(int i = 0; i< currentDialogueData.lines.Length; i++)
        {
            if (currentDialogueData.lines[i].speaker == null) continue;
            if (currentDialogueData.lines[i].speaker.speakerType == SpeakerData.SpeakerType.Player)
            {
                hasPlayer = true;
            }
            else
            {
                hasNPC = true;
            }
            if (hasPlayer && hasNPC) return true;
        }
        return false;
    }

    void SetActive(Image img)
    {
        img.DOColor(Color.white, 0.2f);
    }

    void SetInactive(Image img)
    {
        img.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f);
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
