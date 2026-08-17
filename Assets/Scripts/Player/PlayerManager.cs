using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] DialogueData introDialogue;
    [SerializeField] DialogueData playerTonpc;
    [SerializeField] NPC_Controller npcController;

    private void Start()
    {
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        yield return CoroutineData.GetWaitForSeconds(1f);
        DialogueManager.instance.StartDialogue(introDialogue);
    }

    public void TalkToNpc()
    {
        if (npcController.IsPlayerInRange())
        {
            npcController.Talk();
        }
    }
}
