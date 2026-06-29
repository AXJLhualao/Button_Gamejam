// 2026.6.29 --v1 
// 巡逻状态 —— 实体沿 Path 路径点依次移动。
// 依赖：Path（路径定义）、TargetFollowing（检测目标进入范围）
// 转移条件：
//   - 检测到目标 → 转移到 getChaseState() 返回的状态
//   - 走完所有路径点 → 调用 onPathComplete 回调
// 注入方式：构造函数接收 Func / Action 委托，将状态逻辑与具体 Entity 解耦。
using System;
using UnityEngine;
public class PatrolState : IState
{
    private readonly StateMachine stateMachine;
    private readonly Transform mover;
    private readonly Func<Path> getPath;
    private readonly TargetFollowing targetFollowing;
    private readonly Func<IState> getChaseState;
    private readonly Func<float> getMoveSpeed;
    private readonly Action onPathComplete;
    private readonly Action<Vector3> onMoveDirection;
    private readonly bool reverse;
    private readonly Func<int?> getStartWaypoint;

    private Vector3 targetPosition;
    private int currentWaypoint;

    /// <summary>
    /// 使用固定 Path 创建巡逻状态。
    /// </summary>
    public PatrolState(
        StateMachine stateMachine,
        Transform mover,
        Path path,
        TargetFollowing targetFollowing,
        Func<IState> getChaseState,
        Func<float> getMoveSpeed,
        Action onPathComplete,
        Action<Vector3> onMoveDirection = null,
        bool reverse = false,
        Func<int?> getStartWaypoint = null)
        : this(
            stateMachine,
            mover,
            () => path,
            targetFollowing,
            getChaseState,
            getMoveSpeed,
            onPathComplete,
            onMoveDirection,
            reverse,
            getStartWaypoint)
    {
    }

    /// <summary>
    /// 使用动态 Path 提供器创建巡逻状态。
    /// </summary>
    public PatrolState(
        StateMachine stateMachine,
        Transform mover,
        Func<Path> getPath,
        TargetFollowing targetFollowing,
        Func<IState> getChaseState,
        Func<float> getMoveSpeed,
        Action onPathComplete,
        Action<Vector3> onMoveDirection = null,
        bool reverse = false,
        Func<int?> getStartWaypoint = null)
    {
        this.stateMachine = stateMachine;
        this.mover = mover;
        this.getPath = getPath;
        this.targetFollowing = targetFollowing;
        this.getChaseState = getChaseState;
        this.getMoveSpeed = getMoveSpeed;
        this.onPathComplete = onPathComplete;
        this.onMoveDirection = onMoveDirection;
        this.reverse = reverse;
        this.getStartWaypoint = getStartWaypoint;
    }

    /// <summary>
    /// 进入巡逻状态时决定初始目标路径点。
    /// </summary>
    public void Enter()
    {
        Path path = GetPath();
        if (!HasValidPath(path)) return;

        int defaultWaypoint = reverse ? path.Waypoints.Length - 1 : 0;
        currentWaypoint = Mathf.Clamp(getStartWaypoint?.Invoke() ?? defaultWaypoint, 0, path.Waypoints.Length - 1);

        targetPosition = path.get_position(currentWaypoint);
    }

    /// <summary>
    /// 沿路径点移动，并在发现目标时切换到追击状态。
    /// </summary>
    public void Update()
    {
        if (targetFollowing != null && targetFollowing.HasTarget())
        {
            stateMachine.TransitionTo(getChaseState());
            return;
        }

        Path path = GetPath();
        if (!HasValidPath(path)) return;

        Vector3 moveDirection = (targetPosition - mover.position).normalized;
        onMoveDirection?.Invoke(moveDirection);
        mover.position = Vector3.MoveTowards(mover.position, targetPosition, getMoveSpeed() * Time.deltaTime);

        float relativeDistance = (mover.position - targetPosition).magnitude;
        if (relativeDistance >= 0.1f) return;

        int nextWaypoint = reverse ? currentWaypoint - 1 : currentWaypoint + 1;
        if (nextWaypoint >= 0 && nextWaypoint < path.Waypoints.Length)
        {
            currentWaypoint = nextWaypoint;
            targetPosition = path.get_position(currentWaypoint);
            return;
        }

        onPathComplete?.Invoke();
    }

    /// <summary>
    /// 退出巡逻状态时不需要额外清理。
    /// </summary>
    public void Exit()
    {
    }

    /// <summary>
    /// 获取当前巡逻路径。
    /// </summary>
    private Path GetPath()
    {
        return getPath?.Invoke();
    }

    /// <summary>
    /// 判断路径是否包含可巡逻的路径点。
    /// </summary>
    private bool HasValidPath(Path path)
    {
        return path != null && path.Waypoints != null && path.Waypoints.Length > 0;
    }
}
