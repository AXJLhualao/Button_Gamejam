// 2026.6.29 --v1 
// 移动到最近路径状态 —— 用于在出生点附近寻找最近的路径线段点，并移动到该点。
// 设计模式：状态机（State Machine）
//   - 进入状态时清空上一次的路径目标。
//   - Update 时尝试寻找最近路径点并移动到该点，到位后转移到后续巡逻状态。
//   - Exit 时不需要额外清理。
//
// 用法：
//   IState moveToPath = new MoveToPathState(stateMachine, transform, GetMoveSpeed, GetNextState, OnArrivedPath, OnMoveDirection, reverse: false, arriveDistance: 0.05f);
//   stateMachine.TransitionTo(moveToPath);
using System;
using UnityEngine;

public class MoveToPathState : IState
{
    private readonly StateMachine stateMachine;
    private readonly Transform mover; // 需要移动的 Transform
    private readonly Func<float> getMoveSpeed; // 获取移动速度的委托
    private readonly Func<IState> getNextState; // 获取到达路径后应该转移的下一个状态的委托
    private readonly Action<Path, int> onArrived; // 到达路径后触发的回调，传入路径和起始路径点索引
    private readonly Action<Vector3> onMoveDirection;// 移动方向回调，传入当前移动方向
    private readonly bool reverse; // 是否反向移动到路径点
    private readonly float arriveDistance; // 到达路径点的距离阈值

    private Path targetPath; // 当前目标路径
    private Vector3 targetPoint; // 当前目标路径点
    private int targetSegmentIndex = -1; // 当前目标路径线段索引
    private bool hasTargetPoint; // 是否已经找到目标路径点
    private bool hasWarnedMissingPath; // 是否已经警告过找不到路径

    /// <summary>
    /// 创建移动到最近路径线段点的状态。
    /// </summary>
    public MoveToPathState(
        StateMachine stateMachine,
        Transform mover,
        Func<float> getMoveSpeed,
        Func<IState> getNextState,
        Action<Path, int> onArrived,
        Action<Vector3> onMoveDirection = null,
        bool reverse = false,
        float arriveDistance = 0.05f)
    {
        this.stateMachine = stateMachine;
        this.mover = mover;
        this.getMoveSpeed = getMoveSpeed;
        this.getNextState = getNextState;
        this.onArrived = onArrived;
        this.onMoveDirection = onMoveDirection;
        this.reverse = reverse;
        this.arriveDistance = arriveDistance;
    }

    /// <summary>
    /// 进入上路状态时清空上一次的路径目标。
    /// </summary>
    public void Enter()
    {
        targetPath = null;
        targetPoint = Vector3.zero;
        targetSegmentIndex = -1;
        hasTargetPoint = false;
        hasWarnedMissingPath = false;
    }

    /// <summary>
    /// 移动到最近路径线段点，到位后转移到后续巡逻状态。
    /// </summary>
    public void Update()
    {
        if (!hasTargetPoint && !TryFindNearestPathPoint()) return;

        Vector3 moveDirection = (targetPoint - mover.position).normalized;
        onMoveDirection?.Invoke(moveDirection);
        mover.position = Vector3.MoveTowards(mover.position, targetPoint, getMoveSpeed() * Time.deltaTime);

        if (Vector3.Distance(mover.position, targetPoint) > arriveDistance) return;

        int startWaypoint = GetStartWaypoint();
        onArrived?.Invoke(targetPath, startWaypoint);
        IState nextState = getNextState?.Invoke();
        if (nextState != null)
        {
            stateMachine.TransitionTo(nextState);
        }
    }

    /// <summary>
    /// 退出上路状态时不需要额外清理。
    /// </summary>
    public void Exit()
    {
    }

    /// <summary>
    /// 查找当前出生位置附近最近的路径线段点。
    /// </summary>
    private bool TryFindNearestPathPoint()
    {
        targetPath = PathRegistry.GetNearestPath(mover.position, out targetPoint, out targetSegmentIndex);
        hasTargetPoint = targetPath != null;

        if (!hasTargetPoint && !hasWarnedMissingPath)
        {
            Debug.LogWarning($"{mover.name} could not find a valid Path.");
            hasWarnedMissingPath = true;
        }

        return hasTargetPoint;
    }

    /// <summary>
    /// 根据巡逻方向计算到达路径后应该前往的第一个路径点。
    /// </summary>
    private int GetStartWaypoint()
    {
        if (targetPath == null || targetPath.Waypoints == null || targetPath.Waypoints.Length == 0)
        {
            return 0;
        }

        int startWaypoint = reverse ? targetSegmentIndex : targetSegmentIndex + 1;
        return Mathf.Clamp(startWaypoint, 0, targetPath.Waypoints.Length - 1);
    }
}
