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

    [SerializeField] Image leftCharacterImg;                    //왼쪽에 나타날 sprite
    [SerializeField] Image rightCharacterImg;                   //오른쪽에 나타날 sprite

    DialogueData currentDialogueData;                           //현재 대사 데이터
    int currentIndex;                                           //현재 대사 인덱스
    
    /*
     * Action은 해당하는 함수가 끝날때 코드의 내용을 바꾸지 않고 다음 행동을 하기 위해서 사용되는 편리한 기능이다.
     * 대화 시스템에서 보통 이렇게 작성하는 경우가 많기 때문에 지금 당장은 사용하지 않는다고 하더라도 Action을 넣어주는게
     * 게임 개발의 대화 시스템에서 사용되는 것이다.
     */
    Action onEndCallback;                                       


    private void Awake()
    {
        if (instance != null && instance != this)               //싱글톤으로 하기 위해서
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        talkUI.SetActive(false);                                //처음에는 이야기 ui가 나타나지 않게 하기 위함
    }

    public void StartDialogue(DialogueData data, Action onEnd = null)       //대화의 시작
    {
        currentDialogueData = data;                                         
        currentIndex = 0;
        onEndCallback = onEnd;
        talkUI.SetActive(true);
        /*
         * 대화할떄 필요한 부분 -  처음 대화한다고 생각하자.
         * data에 DialogueData를 넣어서 어떤 정보가 있는지 확인하고, 그것의 처음의 인덱스를 설정해야
         * 대화의 처음을 설정할수가 있으며, 대화 창을 연다.
         */

        ShowCurrentLine();
    }
    
    public void NextLine()
    {
        currentIndex++;
        if(currentIndex >= currentDialogueData.lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    //현재 대화의 상태를 나타내기 위한 함수
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
        talkUI.SetActive(false);
        Action callback = onEndCallback;
        onEndCallback = null;
        callback?.Invoke();
    }
}
