// <summary>
// CombatStats.cs
// 这个类用于存储战斗单位的战斗属性，如移动速度和伤害。
// 被 Enemy 和 Soldier 等战斗单位使用。（以被当作RequireComponent，无需手动挂载）
// </summary>
using UnityEngine;

public class CombatStats : MonoBehaviour
{
    private float moveSpeed;
    private float damage;

    public float MoveSpeed => moveSpeed;
    public float Damage => damage;

    public void InitializeFrom(CombatUnitConfig.SoldierStats stats)
    {
        moveSpeed = stats.moveSpeed;
        damage = stats.damage;
    }

    public void InitializeFrom(EnemyConfig config)
    {
        moveSpeed = config.moveSpeed;
        damage = config.attack;
    }
}
