using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Prefab;
    [SerializeField] private Transform spawnPoint;
    private float spawn_timer;
    [SerializeField] private float spawn_interval = 1f;
    void Update()
    {
        spawn_timer -= Time.deltaTime;
        if (spawn_timer <= 0)
        {
            spawn_timer = spawn_interval;
            SpawnEnemy();
        }
    }
    private void SpawnEnemy()
    {
        GameObject spawnedObject = GameObject.Instantiate(Prefab);
        spawnedObject.transform.position = spawnPoint.position;
    }
}
