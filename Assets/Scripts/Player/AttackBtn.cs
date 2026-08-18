using UnityEngine;
using UnityEngine.UI;

public class AttackBtn : MonoBehaviour
{
    Button attackBtn;
    PlayerAttack playerAttack;

    private void Awake()
    {
        attackBtn = GetComponent<Button>();
        attackBtn.onClick.AddListener(OnAttackClicked);
    }

    private void Update()
    {
        // 아직 참조가 없을 때만 탐색 (찾고 나면 더 이상 Update에서 안 돎)
        if (playerAttack == null)
        {
            TryCheckPlayer();
        }
    }

    void TryCheckPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return; // 아직 스폰 안 됐으면 그냥 넘어감

        PlayerAttack attack = player.GetComponentInChildren<PlayerAttack>();
        if (attack == null) return;

        playerAttack = attack;
    }

    void OnAttackClicked()
    {
        if (playerAttack == null)
        {
            Debug.LogWarning("AttackBtn: PlayerAttack 참조가 아직 없습니다.");
            return;
        }

        playerAttack.Attack();
    }

    private void OnDisable()
    {
        playerAttack = null;
    }
}