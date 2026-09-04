using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("클리어 시 실행할 것들")]
    [SerializeField] SceneDialogueTrigger dialogueTriggerOnClear;
    [SerializeField] GameObject nextRoomGate;
    [SerializeField] bool activateGateOnClear = false;   // true면 켜기, false면 끄기
    [SerializeField] UnityEngine.Events.UnityEvent onRoomCleared;

    int aliveEnemyCount;
    bool isCleared = false;

    public void SetTotalEnemyCount(int count)
    {
        aliveEnemyCount = count;
        Debug.Log($"[RoomManager:{name}] 총 적 수 설정됨: {aliveEnemyCount}");
    }

    public void ReportEnemyDeath()
    {
        Debug.Log($"[RoomManager:{name}] ReportEnemyDeath 호출됨. isCleared={isCleared}, 현재 aliveEnemyCount={aliveEnemyCount}");

        if (isCleared) return;

        aliveEnemyCount--;
        Debug.Log($"[RoomManager:{name}] 적 죽음 카운트 감소 -> 남은 적 수: {aliveEnemyCount}");

        if (aliveEnemyCount <= 0)
        {
            ClearRoom();
        }
    }

    void ClearRoom()
    {
        Debug.Log($"[RoomManager:{name}] ClearRoom 호출됨! 다음 단계 진행");
        isCleared = true;

        if (nextRoomGate != null)
            nextRoomGate.SetActive(activateGateOnClear);   // 체크박스 값에 따라 켜거나 끔

        if (dialogueTriggerOnClear != null)
        {
            Debug.Log($"[RoomManager:{name}] dialogueTriggerOnClear.TriggerDialogue() 호출");
            dialogueTriggerOnClear.TriggerDialogue();
        }
        else
        {
            Debug.LogWarning($"[RoomManager:{name}] dialogueTriggerOnClear가 연결되어 있지 않음!");
        }

        onRoomCleared?.Invoke();
    }
}