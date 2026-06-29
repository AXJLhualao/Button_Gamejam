using System;
using UnityEngine;

[Serializable]
public class WaveEntry
{
    public EnemyConfig enemyConfig;
    public int count = 1;
}

[CreateAssetMenu(menuName = "Combat/Wave Config", fileName = "NewWaveConfig")]
public class WaveConfig : ScriptableObject
{
    public int waveIndex;
    public float spawnInterval = 1.5f;
    public WaveEntry[] entries;
}
