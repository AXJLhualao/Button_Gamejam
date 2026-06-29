using System;
using UnityEngine;

public class EnemyCombat : CombatEntity
{
    [SerializeField] private EnemyConfig config;

    public event Action<EnemyCombat, EnemyConfig> OnDied;

    public EnemyConfig Config => config;

    private Enemy enemy;
    private bool configured;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        if (!configured) ApplyConfig(config);
    }

    public void SetConfig(EnemyConfig enemyConfig)
    {
        config = enemyConfig;
        ApplyConfig(config);
    }

    private void ApplyConfig(EnemyConfig enemyConfig)
    {
        configured = true;
        Stats.InitializeFrom(enemyConfig);
        Health.Initialize(enemyConfig.maxHealth);
    }

    protected override void HandleDeath(DamageInfo damageInfo)
    {
        DieAsEntity(enemy, () => OnDied?.Invoke(this, config));
    }
}
