// <summary>
// TargetFollowing.cs
// 目标追踪组件，管理目标发现、距离检测和追踪逻辑。
// 根据 TargetFinding 发现的目标，控制是否继续搜索。
// </summary> 
using UnityEngine;

[RequireComponent(typeof(TargetFinding))]
public class TargetFollowing : MonoBehaviour
{
    [SerializeField] private GameObject body;
    [SerializeField] private float stop_distance = 0.1f;
    [SerializeField] private bool enable_detection_on_enable = true;

    private TargetFinding targetFinding;
    private Transform target;
    private bool is_close_target;

    public Transform Target => target;

    private void Awake()
    {
        targetFinding = GetComponent<TargetFinding>();
        targetFinding.OnTargetFound += OnTargetFound;
    }

    private void OnDestroy()
    {
        targetFinding.OnTargetFound -= OnTargetFound;
    }

    private void OnTargetFound(Transform newTarget)
    {
        target = newTarget;
        SetCloseTarget(false);
    }

    private void OnEnable()
    {
        targetFinding = GetComponent<TargetFinding>();
        targetFinding.SetActive(enable_detection_on_enable);
    }

    private void OnDisable()
    {
        targetFinding.SetActive(false);
    }

    public void SetEnableDetectionOnEnable(bool enable)
    {
        enable_detection_on_enable = enable;
    }

    public void SetDetectionActive(bool active)
    {
        targetFinding = GetComponent<TargetFinding>();
        targetFinding.SetActive(active);
    }

    public Vector3 MoveTowardsTarget(float speed)
    {
        Vector3 direction = (target.position - body.transform.position).normalized;
        body.transform.position += direction * speed * Time.deltaTime;
        SetCloseTarget(IsCloseToTarget());
        return direction;
    }

    public bool IsCloseToTarget()
    {
        return Vector3.Distance(body.transform.position, target.position) < stop_distance;
    }

    private void SetCloseTarget(bool isClose)
    {
        if (is_close_target == isClose) return;

        is_close_target = isClose;
        targetFinding.SetActive(!is_close_target);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stop_distance);
    }

    public bool HasTarget()
    {
        if (target == null) return false;
        if (target.GetComponentInParent<Health>().IsDead){
            ClearTarget();
            return false;
        }
        return true;
    }

    public void ClearTarget()
    {
        target = null;
        SetCloseTarget(false);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        SetCloseTarget(false);
    }
}
