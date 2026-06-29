// <summary>
// 目标检测组件：在指定范围内检测目标并触发事件。
// 通过设置检测半径和目标层，可以在Update中持续检测目标。
// 用法：
// 1. 将此组件附加到需要检测目标的GameObject上。
// 2. 设置检测半径和目标层。
// 3. 订阅OnTargetFound事件以处理检测到的目标。 
// </summary>

using UnityEngine;
using System;

public class TargetFinding : MonoBehaviour
{
    [SerializeField] private bool isActive = true;
    [SerializeField] private LayerMask target_layer;
    [SerializeField] private float detection_radius = 10f;

    public event Action<Transform> OnTargetFound;

    public void SetActive(bool active)
    {
        isActive = active;
    }

    void Update()
    {
        if (!isActive) return;
        Detect();
    }
    // <summary>
    // 检测范围内的目标，并触发OnTargetFound事件。
    // </summary>
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
    // <summary>
    // 检查是否可以检测到目标。
    // 1. 自身和目标实体都不为空。
    // 2. 自身和目标实体不是同一个对象。
    // 3. 自身和目标实体在同一路径上。
    // 4. 自身和目标实体不属于同一队伍。
    // 5. 目标实体的Health组件不存在或未死亡。
    // </summary>
    private bool CanDetectTarget(Entity selfEntity, Entity targetEntity)
    {
        if (selfEntity == null || targetEntity == null) return false; // 
        if (selfEntity == targetEntity) return false;
        if (!selfEntity.IsOnSamePath(targetEntity)) return false;

        TeamMember selfTeam = selfEntity.GetComponent<TeamMember>();
        TeamMember targetTeam = targetEntity.GetComponent<TeamMember>();
        if (selfTeam.Team == targetTeam.Team) return false;

        Health targetHealth = targetEntity.GetComponent<Health>();
        return targetHealth == null || !targetHealth.IsDead;
    }
    // <summary>
    // 在编辑器中绘制检测范围的Gizmos。
    // </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detection_radius);
    }
}
