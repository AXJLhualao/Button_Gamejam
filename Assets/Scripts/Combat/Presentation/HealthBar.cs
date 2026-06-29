using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        health.OnHealthChanged += UpdateBar;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(float currentHealth)
    {
        fillImage.fillAmount = currentHealth / health.MaxHealth;
    }
}
