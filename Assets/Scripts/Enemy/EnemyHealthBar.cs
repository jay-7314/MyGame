using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Enemy enemy;   

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (enemy == null) enemy = GetComponentInParent<Enemy>();
    }

    private void OnEnable()
    {
        if (enemy != null)
        {
            enemy.OnHealthChanged += UpdateHealthBar;
        }
    }

    private void OnDisable()
    {
        if (enemy != null)
        {
            enemy.OnHealthChanged -= UpdateHealthBar;
        }
    }

    void UpdateHealthBar(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}