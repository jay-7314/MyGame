using System.Collections;
using UnityEngine;

public class SceneDialogueTrigger : MonoBehaviour
{
    [SerializeField] DialogueData[] candidates;
    [SerializeField] float delay = 1f;

    [Header("시작 방식")]
    [SerializeField] bool autoStartOnSceneLoad = true;   // 씬 시작하자마자 자동으로 시작할지

    [Header("대사 종료 시 오브젝트 활성/비활성 처리")]
    [SerializeField] GameObject objectToActivateOnEnd;
    [SerializeField] GameObject objectToDeactivateOnEnd;

    bool hasTriggered = false;   // 중복 실행 방지

    void Start()
    {
        if (autoStartOnSceneLoad)
        {
            StartCoroutine(DelayedTrigger());
        }
    }

    IEnumerator DelayedTrigger()
    {
        yield return CoroutineData.GetWaitForSeconds(delay);
        TriggerDialogue();
    }

    // 외부(RoomManager 등)에서 원하는 타이밍에 직접 호출하는 진입점
    public void TriggerDialogue()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        DialogueData best = null;
        for (int i = 0; i < candidates.Length; i++)
        {
            DialogueData dialogueData = candidates[i];
            if (!IsUnlocked(dialogueData)) continue;
            if (best == null || dialogueData.priority > best.priority)
                best = dialogueData;
        }

        if (best != null)
            DialogueManager.instance.StartDialogue(best, OnDialogueEnd);
        else
            hasTriggered = false;   // 조건에 맞는 대사가 없었으면 다시 시도 가능하게 풀어줌
    }

    void OnDialogueEnd()
    {
        if (objectToDeactivateOnEnd != null) objectToDeactivateOnEnd.SetActive(false);
        if (objectToActivateOnEnd != null) objectToActivateOnEnd.SetActive(true);
    }

    bool IsUnlocked(DialogueData dialogue)
    {
        if (GameFlags.instance == null) return true;
        if (dialogue.requiredFlags != null)
        {
            for (int i = 0; i < dialogue.requiredFlags.Length; i++)
            {
                if (!GameFlags.instance.hasFlag(dialogue.requiredFlags[i]))
                    return false;
            }
        }
        if (dialogue.blockedIfFlags != null)
        {
            for (int i = 0; i < dialogue.blockedIfFlags.Length; i++)
            {
                if (GameFlags.instance.hasFlag(dialogue.blockedIfFlags[i]))
                    return false;
            }
        }
        return true;
    }
}