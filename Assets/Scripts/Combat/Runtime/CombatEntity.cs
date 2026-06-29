using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CombatStats))]
[RequireComponent(typeof(TeamMember))]
public abstract class CombatEntity : MonoBehaviour
{
    [SerializeField] protected TargetFollowing targetFollowing;

    protected Health Health { get; private set; }
    protected CombatStats Stats { get; private set; }
    
    protected virtual void Awake()
    {
        Health = GetComponent<Health>();
        Stats = GetComponent<CombatStats>();
        Health.OnDied += HandleDeath;
    }

    protected virtual void OnDestroy()
    {
        if (Health != null)
            Health.OnDied -= HandleDeath;
    }

    protected abstract void HandleDeath(DamageInfo damageInfo);

    public void PerformAttack()
    {
        if (targetFollowing == null || !targetFollowing.HasTarget()) return;
        Transform target = targetFollowing.Target;
        if (target == null) return;

        Health targetHealth = target.GetComponentInParent<Health>();
        if (targetHealth == null || targetHealth.IsDead) return;

        Entity attacker = GetComponent<Entity>();
        DamageInfo info = new DamageInfo(Stats.Damage, DamageType.Physical, attacker);
        targetHealth.TakeDamage(info);
    }

    protected void DieAsEntity(Entity entity, Action onDied, Action extraDisable = null)
    {
        onDied?.Invoke();

        void Disable()
        {
            foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
                col.enabled = false;

            extraDisable?.Invoke();
            entity.enabled = false;
        }

        entity.EnterDeathState(Disable);
    }
}
