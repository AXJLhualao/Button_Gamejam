using System.Numerics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float movespeed = 3;
    [SerializeField] private Path current_path;
    private UnityEngine.Vector3 target_position;
    private int current_waypoint;
    private void Awake()
    {
        current_path = GameObject.Find("Path1").GetComponent<Path>();
    }
    private void OnEnable()
    {
        current_waypoint = 0;
        target_position = current_path.get_position(current_waypoint);
    }
    void Update()
    {
        //前往目标地点
        transform.position = UnityEngine.Vector3.MoveTowards(transform.position, target_position, movespeed * Time.deltaTime);
        float relative_distance = (transform.position - target_position).magnitude;
        if (relative_distance < 0.1f)
        {
            if (current_waypoint < current_path.Waypoints.Length - 1)
            {
                current_waypoint++;
                target_position = current_path.get_position(current_waypoint);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
