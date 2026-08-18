using System.Collections;
using UnityEngine;

public class SceneDialogueTrigger : MonoBehaviour
{
    [SerializeField] DialogueData[] candidates;
    [SerializeField] float delay = 1f;

    IEnumerator Start()
    {
        yield return CoroutineData.GetWaitForSeconds(delay);

        DialogueData best = null;
        for (int i = 0; i < candidates.Length; i++)
        {
            DialogueData dialogueData = candidates[i];
            if (!IsUnlocked(dialogueData)) continue;
            if (best == null || dialogueData.priority > best.priority)
                best = dialogueData;
        }

        if (best != null)
            DialogueManager.instance.StartDialogue(best);
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