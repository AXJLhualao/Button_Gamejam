// 2026.6.29 --v1 
// 攻击状态 —— 维持攻击姿态，实际伤害由 Attack 动画事件触发（CombatAnimationEvents.OnAttackHit）。
using System;

public class AttackState : IState
{
    private readonly StateMachine stateMachine;
    private readonly Func<bool> hasTarget;
    private readonly Func<bool> shouldKeepAttacking;
    private readonly Func<IState> getChaseState;

    public AttackState(
        StateMachine stateMachine,
        Func<bool> hasTarget,
        Func<bool> shouldKeepAttacking,
        Func<IState> getChaseState)
    {
        this.stateMachine = stateMachine;
        this.hasTarget = hasTarget;
        this.shouldKeepAttacking = shouldKeepAttacking;
        this.getChaseState = getChaseState;
    }

    /// <summary>
    /// 进入攻击状态，等待动画事件结算伤害。
    /// </summary>
    public void Enter()
    {
    }

    /// <summary>
    /// 检查目标是否仍有效，无效则退回追击。
    /// </summary>
    public void Update()
    {
        if (!hasTarget() || !shouldKeepAttacking()) TransitionToChase();
    }

    /// <summary>
    /// 退出攻击状态时无需额外清理。
    /// </summary>
    public void Exit()
    {
    }

    /// <summary>
    /// 切换回追击状态。
    /// </summary>
    private void TransitionToChase()
    {
        IState chaseState = getChaseState?.Invoke();
        stateMachine.TransitionTo(chaseState);
    }
}
