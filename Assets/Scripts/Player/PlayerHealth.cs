using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public event System.Action<float, float> OnHealthChanged;

    [Header("UI - 캐릭터 머리 위 체력바 (프리팹 내부)")]
    [SerializeField] Slider healthSlider;

    [Header("Animation")]
    [SerializeField] Animator anim;

    [Header("Die")]
    [SerializeField] float dieDelay = 1.5f;
    [SerializeField] DialogueData deathDialogue; // 죽었을 때 띄울 대사 데이터

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
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        UpdateHealthUI();


        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            anim.SetTrigger(HitTriggerParam);
            StartInvincibility();

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
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
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
    }

    public void OnDeathAnimationComplete()
    {
        FinishDeath();
    }

    void FinishDeath()
    {
        if (deathDialogue != null && DialogueManager.instance != null)
        {
            DialogueManager.instance.StartDialogue(deathDialogue, OnDeathDialogueEnd);
        }
    }

    void OnDeathDialogueEnd()
    {
        SceneManager.LoadScene("intro");
    }
}