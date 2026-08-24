using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;

    [Header("UI - 캐릭터 머리 위 체력바 (프리팹 내부)")]
    [SerializeField] Slider healthSlider;

    [Header("UI - 화면 고정 HUD 체력바")]
    [SerializeField] Slider screenHealthSlider; // 씬에 있는 HUD Slider. 못 찾으면 태그로 자동 검색

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

        // 화면 HUD Slider가 Inspector에서 연결 안 되어 있으면 태그로 자동 검색
        if (screenHealthSlider == null)
        {
            GameObject hudObj = GameObject.FindWithTag("HealthBarUI");
            if (hudObj != null)
            {
                screenHealthSlider = hudObj.GetComponent<Slider>();
            }
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (screenHealthSlider != null)
        {
            screenHealthSlider.maxValue = maxHealth;
            screenHealthSlider.value = currentHealth;
        }
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

        if (screenHealthSlider != null)
        {
            screenHealthSlider.value = currentHealth;
        }
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