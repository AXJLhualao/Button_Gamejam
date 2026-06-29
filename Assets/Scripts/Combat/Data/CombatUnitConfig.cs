using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Combat/Combat Unit Config", fileName = "NewCombatUnitConfig")]
public class CombatUnitConfig : ScriptableObject
{
    [Serializable]
    public class SoldierStats
    {
        public float maxHealth = 80f;
        public float moveSpeed = 3f;
        public float damage = 12f;
    }

    public string id;
    public string displayName;
    public int buildCost = 50;

    [Header("步兵营")]
    public GameObject soldierPrefab;

    [FormerlySerializedAs("baseSoldierStats")]
    public SoldierStats soldierStats = new SoldierStats();

    public int maxSoldierCapacity = 3;
    public float spawnInterval = 3f;
}
