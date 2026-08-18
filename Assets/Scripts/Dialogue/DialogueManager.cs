using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        if (choicePanel != null) choicePanel.SetActive(false);
    }

    public void StartDialogue(DialogueData data, Action onEnd = null)
    {
        StartDialogue(data, 0, onEnd);
    }

    public void StartDialogue(DialogueData data, int startIndex, Action onEnd = null)
    {
        currentDialogueData = data;
        currentIndex = startIndex;
        onEndCallback = onEnd;
        isDialogueCheck = true;
        talkUI.SetActive(true);
        ShowCurrentLine();
    }

    public void NextLine()
    {
        if (!isDialogueCheck) return;

        DialogueLine currentLine = currentDialogueData.lines[currentIndex];
        if (currentLine.choices != null && currentLine.choices.Length > 0) return;

        if (currentLine.isNextScene)
        {
            EndDialogue();
            GoToNextScene("Map"); // 원하시는 씬 이름
            return;
        }

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
        bool hasImg = line.speaker.speakerImg != null;

        if (line.speaker.speakerType == SpeakerData.SpeakerType.Player)
        {
            if (leftCharacterImg != null)
            {
                leftCharacterImg.gameObject.SetActive(hasImg);
                if (hasImg)
                {
                    leftCharacterImg.sprite = line.speaker.speakerImg;
                    SetActive(leftCharacterImg);
                }
            }
            if (rightCharacterImg != null)
            {
                rightCharacterImg.gameObject.SetActive(isTwoSpeakers);
                if (isTwoSpeakers) SetInactive(rightCharacterImg);
            }
        }
        else
        {
            if (rightCharacterImg != null)
            {
                rightCharacterImg.gameObject.SetActive(hasImg);
                if (hasImg)
                {
                    rightCharacterImg.sprite = line.speaker.speakerImg;
                    SetActive(rightCharacterImg);
                }
            }
            if (leftCharacterImg != null)
            {
                leftCharacterImg.gameObject.SetActive(isTwoSpeakers);
                if (isTwoSpeakers) SetInactive(leftCharacterImg);
            }
        }

        dialogue.text = line.dialogueText;

        if (line.choices != null && line.choices.Length > 0)
            ShowChoices(line.choices);
        else if (choicePanel != null)
            choicePanel.SetActive(false);
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
        img.DOKill();
        img.color = Color.white;
    }

    void SetInactive(Image img)
    {
        img.DOKill();
        img.color = new Color(0.5f, 0.5f, 0.5f);
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

        if (choice.setFlagOnSelect != GameFlagKey.None && GameFlags.instance != null)
            GameFlags.instance.setFlag(choice.setFlagOnSelect);

        if (choice.isNextScene)
        {
            EndDialogue();
            GoToNextScene("Map");
            return;
        }

        if (choice.nextDialogue != null)
            StartDialogue(choice.nextDialogue, onEndCallback);
        else
            EndDialogue();
    }

    public void GoToNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}