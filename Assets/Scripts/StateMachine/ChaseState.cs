// 2026.6.29 --v1 
// 追逐状态 —— 实体向目标移动，直到满足攻击条件或丢失目标。
// 依赖：TargetFollowing（提供位置更新与距离判断）
// 转移条件：
//   - 丢失目标 → 转移到 getNoTargetState()（通常为 PatrolState）
//   - 进入攻击范围 → 转移到 getAttackState()（由 shouldEnterAttack 判定）
// 注意：构造函数将 TargetFollowing 的返回值直接作为移动方向返回，
//       方便上层 Entity 做动画朝向同步。
using System;
using UnityEngine;

public class ChaseState : IState
{
    private readonly StateMachine stateMachine;
    private readonly TargetFollowing targetFollowing;
    private readonly Func<IState> getNoTargetState;
    private readonly Func<float> getMoveSpeed;
    private readonly Func<IState> getAttackState;
    private readonly Func<bool> shouldEnterAttack;
    private readonly Action<Vector3> onMoveDirection;

    public ChaseState(
        StateMachine stateMachine,
        TargetFollowing targetFollowing,
        Func<IState> getNoTargetState,
        Func<float> getMoveSpeed,
        Func<IState> getAttackState = null,
        Func<bool> shouldEnterAttack = null,
        Action<Vector3> onMoveDirection = null)
    {
        this.stateMachine = stateMachine;
        this.targetFollowing = targetFollowing;
        this.getNoTargetState = getNoTargetState;
        this.getMoveSpeed = getMoveSpeed;
        this.getAttackState = getAttackState;
        this.shouldEnterAttack = shouldEnterAttack;
        this.onMoveDirection = onMoveDirection;
    }

    public void Enter()
    {
    }

    public void Update()
    {
        if (!targetFollowing.HasTarget())
        {
            targetFollowing.ClearTarget();
            IState noTargetState = getNoTargetState?.Invoke();
            stateMachine.TransitionTo(noTargetState);
            return;
        }

        Vector3 moveDirection = targetFollowing.MoveTowardsTarget(getMoveSpeed());
        onMoveDirection?.Invoke(moveDirection);

        if (shouldEnterAttack())
        {
            IState attackState = getAttackState?.Invoke();
            stateMachine.TransitionTo(attackState);
        }
    }

    public void Exit()
    {
    }
}
