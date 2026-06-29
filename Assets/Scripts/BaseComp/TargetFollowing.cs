// 2026.6.29 --v1 
// 目标跟随组件 —— 驱动 body 向 target 移动，并提供距离状态通知。
// 依赖：TargetFinding（通过 OnTargetFound 事件接收目标引用）
// 设计要点：
//   - MoveTowardsTarget 返回归一化方向向量，方便调用方做动画朝向同步
//   - 接近目标（IsCloseToTarget）后自动停用 TargetFinding，减少不必要的检测
//   - 通过 OnCloseTargetChanged 事件通知其他系统（如 AttackState 的条件判定）
// 注意：body 字段允许将移动对象与触发器对象分离（子物体 → 父物体移动）。
using System;
using UnityEngine;
[RequireComponent(typeof(TargetFinding))]
public class TargetFollowing : MonoBehaviour
{
    [SerializeField] private TargetFinding targetFinding => GetComponent<TargetFinding>();
    [SerializeField] private GameObject body;
    [SerializeField] private Transform target;
    [SerializeField] private bool is_close_target = false;
    [SerializeField] private float stop_distance = 0.1f;
    [SerializeField] private bool enable_detection_on_enable = true;

    public event Action<bool> OnCloseTargetChanged;
    public bool IsCloseTarget => is_close_target;

    /// <summary>
    /// 初始化时订阅目标发现事件。
    /// </summary>
    private void Awake()
    {
        targetFinding.OnTargetFound += OnTargetFound;
    }

    /// <summary>
    /// 销毁时取消订阅目标发现事件。
    /// </summary>
    private void OnDestroy()
    {
        targetFinding.OnTargetFound -= OnTargetFound;
    }

    /// <summary>
    /// 接收检测组件发现的新目标。
    /// </summary>
    private void OnTargetFound(Transform newTarget)
    {
        target = newTarget;
        SetCloseTarget(false);
    }

    /// <summary>
    /// 组件启用时按配置决定是否开启检测。
    /// </summary>
    private void OnEnable() => targetFinding.set_active(enable_detection_on_enable);

    /// <summary>
    /// 组件禁用时关闭目标检测。
    /// </summary>
    private void OnDisable() => targetFinding.set_active(false);

    /// <summary>
    /// 设置启用组件时是否自动开启目标检测。
    /// </summary>
    public void SetEnableDetectionOnEnable(bool enable)
    {
        enable_detection_on_enable = enable;
    }

    /// <summary>
    /// 手动开启或关闭目标检测。
    /// </summary>
    public void SetDetectionActive(bool active)
    {
        targetFinding.set_active(active);
    }

    /// <summary>
    /// 向当前目标移动，并返回本帧移动方向。
    /// </summary>
    public Vector3 MoveTowardsTarget(float speed)
    {
        if (target == null || body == null) return Vector3.zero;

        Vector3 direction = (target.position - body.transform.position).normalized;
        body.transform.position += direction * speed * Time.deltaTime;
        SetCloseTarget(IsCloseToTarget());
        return direction;
    }

    /// <summary>
    /// 判断当前移动体是否已经进入停止距离。
    /// </summary>
    public bool IsCloseToTarget()
    {
        if (target == null || body == null) return false;

        float distance = Vector3.Distance(body.transform.position, target.position);
        return distance < stop_distance;
    }

    /// <summary>
    /// 更新接近目标状态，并同步检测开关。
    /// </summary>
    private void SetCloseTarget(bool isClose)
    {
        if (is_close_target == isClose) return;

        is_close_target = isClose;
        targetFinding.set_active(!is_close_target);
        OnCloseTargetChanged?.Invoke(is_close_target);
    }

    /// <summary>
    /// 在编辑器中绘制停止距离。
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stop_distance);
    }

    /// <summary>
    /// 判断当前是否持有目标。
    /// </summary>
    public bool HasTarget() => target != null;

    /// <summary>
    /// 返回当前目标对象。
    /// </summary>
    public GameObject GetTarget() => target.gameObject;
}