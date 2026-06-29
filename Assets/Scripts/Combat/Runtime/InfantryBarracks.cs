// <summary>
// InfantryBarracks.cs
// 这个类用于管理步兵兵营的逻辑，包括生成士兵和处理士兵死亡事件。
// </summary>

using UnityEngine;

public class InfantryBarracks : MonoBehaviour
{
    [SerializeField] private CombatUnitConfig config;
    [SerializeField] private Transform spawnPoint;
    private float spawnTimer;
    private int activeSoldiers;
    private void Update()
    {
        if (activeSoldiers >= config.maxSoldierCapacity) return;
        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f) return;
        SpawnSoldier();
        spawnTimer = config.spawnInterval;
    }
    // <summary>
    // 这个方法用于生成一个新的士兵，并初始化其战斗属性。
    // 它会在指定的生成点实例化士兵预制体，并订阅士兵的OnDied事件，以便在士兵死亡时更新活跃士兵数量。
    // </summary>
    private void SpawnSoldier()
    {
        GameObject soldierObj = Instantiate(config.soldierPrefab, spawnPoint.position, Quaternion.identity);
        SoldierCombat combat = soldierObj.GetComponent<SoldierCombat>();
        combat.Initialize(config.soldierStats);
        combat.OnDied += HandleSoldierDied;
        activeSoldiers++;
    }
    // <summary>
    // 这个方法用于处理士兵死亡事件。
    // 当士兵死亡时，它会取消订阅士兵的OnDied事件，并将活跃士兵数量减一，确保不会出现负数。
    // </summary
    private void HandleSoldierDied(SoldierCombat combat)
    {
        combat.OnDied -= HandleSoldierDied;
        activeSoldiers = Mathf.Max(0, activeSoldiers - 1);
    }
}
