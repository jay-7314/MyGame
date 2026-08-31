using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HudHealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;           
    [SerializeField] TextMeshProUGUI healthText;  
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
        UpdateBar(target.CurrentHealth, target.MaxHealth); 
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