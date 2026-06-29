// <summary>
// WaveManager.cs
// 这个类用于管理敌人波次的生成逻辑。
// 它会根据WaveConfig中的配置生成敌人，并在所有敌人死亡后触发OnWaveCompleted事件。
// </summary>


using System;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private WaveConfig waveConfig;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private bool autoStart = true;
    public event Action OnWaveStarted;
    public event Action OnWaveCompleted;
    private int aliveCount;

    private void Start()
    {
        if (autoStart) StartWave();
    }
    public void StartWave()
    {
        StartCoroutine(SpawnWave());
    }
    // <summary>
    // 这个协程用于生成敌人波次。
    // 它会根据WaveConfig中的配置生成敌人，并在所有敌人死亡后触发OnWaveCompleted事件。
    // </summary>
    private IEnumerator SpawnWave()
    {
        OnWaveStarted?.Invoke();
        aliveCount = 0;

        foreach (var entry in waveConfig.entries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                SpawnEnemy(entry.enemyConfig);
                aliveCount++;
                yield return new WaitForSeconds(waveConfig.spawnInterval);
            }
        }
    }
    // <summary>
    // 这个方法用于生成一个敌人。
    // 它会在指定的生成点实例化敌人预制体，并订阅敌人的OnDied事件，以便在敌人死亡时更新存活敌人数量。
    // </summary>
    private void SpawnEnemy(EnemyConfig config)
    {
        EnemyCombat combat = Instantiate(config.prefab, spawnPoint.position, Quaternion.identity).GetComponent<EnemyCombat>();
        combat.SetConfig(config);
        combat.OnDied += HandleEnemyDied;
    }
    // <summary>
    // 这个方法用于处理敌人死亡事件。
    // 当敌人死亡时，它会取消订阅敌人的OnDied事件，并将存活敌人数量减一。
    // 如果存活敌人数量为零，它会触发OnWaveCompleted事件，并将奖励金币添加到资源管理器中。
    // </summary>
    private void HandleEnemyDied(EnemyCombat combat, EnemyConfig config)
    {
        combat.OnDied -= HandleEnemyDied;
        aliveCount--;
        resourceManager.AddGold(config.rewardGold);

        if (aliveCount <= 0)
            OnWaveCompleted?.Invoke();
    }
}
