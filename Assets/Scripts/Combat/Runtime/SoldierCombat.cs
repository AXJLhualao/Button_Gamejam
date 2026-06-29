// <summary>
// SoldierCombat.cs
// 步兵Soldier的战斗逻辑。处理初始化、攻击、死亡事件。
// </summary>

using System;
using UnityEngine;

public class SoldierCombat : CombatEntity
{
    [SerializeField] private CombatUnitConfig.SoldierStats defaultStats;
    [SerializeField] private Soldier soldier;

    public event Action<SoldierCombat> OnDied;
    private bool initialized;

    private void Start()
    {
        if (!initialized) Initialize(defaultStats);
    }

    public void Initialize(CombatUnitConfig.SoldierStats stats)
    {
        initialized = true;
        Stats.InitializeFrom(stats);
        Health.Initialize(stats.maxHealth);
    }

    protected override void HandleDeath(DamageInfo damageInfo)
    {
        DieAsEntity(
            soldier,
            () => OnDied?.Invoke(this),
            () => targetFollowing?.SetTarget(null));
    }
}
