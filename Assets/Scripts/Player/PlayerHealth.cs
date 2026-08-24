using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;

    // ↓↓↓ 외부에서 읽을 수 있도록 프로퍼티 추가
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    // ↓↓↓ 체력 변경 이벤트 추가 (current, max)
    public event System.Action<float, float> OnHealthChanged;

    [Header("UI - 캐릭터 머리 위 체력바 (프리팹 내부)")]
    [SerializeField] Slider healthSlider;
    [Header("Animation")]
    [SerializeField] Animator anim;
    [Header("Die")]
    [SerializeField] float dieDelay = 1.5f;
    [Header("Invincibility")]
    [SerializeField] float invincibleDuration = 0.5f;
    bool isInvincible = false;
    static readonly int HitTriggerParam = Animator.StringToHash("HitTrigger");
    static readonly int IsDeadParam = Animator.StringToHash("isDead");
    bool isDead = false;
    PlayerController playerController;

    void Awake()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // 초기값도 이벤트로 한번 알려줌 (HUD가 먼저 구독했을 경우 대비)
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (isInvincible) return;
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        UpdateHealthUI();
        Debug.Log($"플레이어가 데미지 {damage}를 입었습니다. 남은 체력: {currentHealth}");
        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            anim.SetTrigger(HitTriggerParam);
            StartInvincibility();

            // 공격 중이었다면 강제로 취소해서 isAttacking이 영구히 남지 않게 함
            var playerAttack = GetComponent<PlayerAttack>();
            if (playerAttack != null)
                playerAttack.ForceCancelAttack();
        }
    }

    void StartInvincibility()
    {
        isInvincible = true;
        CancelInvoke(nameof(EndInvincibility));
        Invoke(nameof(EndInvincibility), invincibleDuration);
    }

    void EndInvincibility()
    {
        isInvincible = false;
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth); // 추가
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        anim.SetBool(IsDeadParam, true);
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        Invoke(nameof(FinishDeath), dieDelay);
    }

    void FinishDeath()
    {
        Debug.Log("플레이어 사망 처리 - 추후 구현 예정");
    }
}