// 2026.6.29 --v1 
// 目标检测组件 —— 使用 Physics2D.OverlapCircleAll 扫描周围目标。
// 设计：事件驱动而非轮询判断。
//   - 检测到目标时触发 OnTargetFound 事件（供 TargetFollowing 订阅）
//   - 每次检测到目标后自动禁用自身（isActive = false），避免重复报告
// 依赖：需在 Inspector 中指定 target_layer 以过滤目标图层。
// 关联组件：TargetFollowing（订阅 OnTargetFound 并驱动移动）。
using UnityEngine;
using System;
public class TargetFinding : MonoBehaviour
{
    [SerializeField] private bool isActive = true;
    [SerializeField] private LayerMask target_layer;
    [SerializeField] private float detection_radius = 10f;

    public event Action<Transform> OnTargetFound;

    /// <summary>
    /// 设置目标检测是否启用。
    /// </summary>
    public void set_active(bool new_active) { isActive = new_active; }

    /// <summary>
    /// 每帧在启用时尝试扫描目标。
    /// </summary>
    void Update()
    {
        if (!isActive) return;
        Detect();
    }

    /// <summary>
    /// 扫描范围内候选目标，并只接受同一路径上的实体。
    /// </summary>
    private void Detect()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detection_radius, target_layer);
        Entity selfEntity = GetComponentInParent<Entity>();

        foreach (var hitCollider in hitColliders)
        {
            Entity targetEntity = hitCollider.GetComponentInParent<Entity>();
            if (!CanDetectTarget(selfEntity, targetEntity)) continue;

            OnTargetFound?.Invoke(targetEntity.transform);
            isActive = false;
            break;
        }
    }

    /// <summary>
    /// 判断候选目标是否是同一路径上的其他实体。
    /// </summary>
    private bool CanDetectTarget(Entity selfEntity, Entity targetEntity)
    {
        if (selfEntity == null || targetEntity == null) return false;
        if (selfEntity == targetEntity) return false;

        return selfEntity.IsOnSamePath(targetEntity);
    }

    /// <summary>
    /// 在编辑器中绘制检测范围。
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detection_radius);
    }
}