using DG.Tweening;
using System;
using TMPro;
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

    [SerializeField] GameObject choicePanel;
    [SerializeField] Button choiceButtonPrefab;

    DialogueData currentDialogueData;                           //현재 대사 데이터
    int currentIndex;                                                          //현재 대사 인덱스
    Action onEndCallback;                                                //대사 종료시 호출할 콜백??
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
        choicePanel.SetActive(false);
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

        // 현재 줄이 선택지 줄이면 무시 (버튼으로만 진행)
        DialogueLine currentLine = currentDialogueData.lines[currentIndex];
        if (currentLine.choices != null && currentLine.choices.Length > 0) return;

        currentIndex++;
        if (currentIndex >= currentDialogueData.lines.Length)
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
            if (isTwoSpeakers) SetInactive(rightCharacterImg);
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

        // 이 줄에 선택지가 있으면 버튼 표시, 없으면 선택지 창 끄기
        if (line.choices != null && line.choices.Length > 0)
        {
            ShowChoices(line.choices);
        }
        else
        {
            choicePanel.SetActive(false);
        }
    }

    bool HasMultiSpeaker()
    {
        bool hasPlayer = false;
        bool hasNPC = false;

        for (int i = 0; i < currentDialogueData.lines.Length; i++)
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

    void ShowChoices(DialogueChoice[] choices)
    {
        choicePanel.SetActive(true);

        for (int i = choicePanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(choicePanel.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < choices.Length; i++)
        {
            DialogueChoice choice = choices[i];
            Button newButton = Instantiate(choiceButtonPrefab, choicePanel.transform);
            newButton.GetComponentInChildren<TMP_Text>().text = choice.choiceText;
            newButton.onClick.AddListener(() => OnChoiceClicked(choice));
        }
    }

    void OnChoiceClicked(DialogueChoice choice)
    {
        choicePanel.SetActive(false);
        StartDialogue(choice.nextDialogue, onEndCallback);
    }
}