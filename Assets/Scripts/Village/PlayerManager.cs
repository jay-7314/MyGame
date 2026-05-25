using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] DialogueData introDialogue;

    private void Start()
    {
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        yield return CoroutineData.GetWaitForSeconds(1f);
        DialogueManager.instance.StartDialogue(introDialogue);
    }
}
