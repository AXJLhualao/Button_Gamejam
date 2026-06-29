using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Enemy Config", fileName = "NewEnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public string id;
    public string displayName;
    public float maxHealth;
    public float moveSpeed;
    public float attack;
    public int rewardGold;
    public GameObject prefab;
}
