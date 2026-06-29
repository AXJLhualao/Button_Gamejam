using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float currentHealth;
    private float maxHealth;

    public event Action<float> OnHealthChanged;
    public event Action<DamageInfo> OnDied;
    public event Action<DamageInfo> OnTakeDamage;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    /// <summary>
    /// 用指定最大生命值重置当前血量。
    /// </summary>
    public void Initialize(float health)
    {
        maxHealth = health;
        currentHealth = health;
    }

    /// <summary>
    /// 扣除伤害并在血量归零时触发死亡事件。
    /// </summary>
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead) return;

        currentHealth -= damageInfo.Amount;
        OnHealthChanged?.Invoke(currentHealth);
        OnTakeDamage?.Invoke(damageInfo);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDied?.Invoke(damageInfo);
        }
    }

    /// <summary>
    /// 恢复生命值，不超过上限。
    /// </summary>
    public void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = value;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
}
