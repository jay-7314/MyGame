using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HudHealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;              // Bar_MP 자기 자신의 Slider
    [SerializeField] TextMeshProUGUI healthText;  // "150 / 150" 표시할 텍스트
    PlayerHealth target;

    void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        StartCoroutine(FindAndBindRoutine());
    }

    IEnumerator FindAndBindRoutine()
    {
        PlayerHealth found = null;

        while (found == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                found = player.GetComponent<PlayerHealth>();
            }

            if (found == null)
                yield return null;
        }

        target = found;
        UpdateBar(target.CurrentHealth, target.MaxHealth); // 초기값 반영
        target.OnHealthChanged += UpdateBar;
    }

    void UpdateBar(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }
    }

    void OnDisable()
    {
        if (target != null)
            target.OnHealthChanged -= UpdateBar;
    }
}