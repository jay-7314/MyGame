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
        if (playerAttack == null)
        {
            TryCheckPlayer();
        }
    }

    void TryCheckPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return; 

        PlayerAttack attack = player.GetComponentInChildren<PlayerAttack>();
        if (attack == null) return;

        playerAttack = attack;
    }

    void OnAttackClicked()
    {
        playerAttack.Attack();
    }

    private void OnDisable()
    {
        playerAttack = null;
    }
}