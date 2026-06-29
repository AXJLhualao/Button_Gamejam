// 2026.6.29 --v1 
// 攻击状态 —— 持续执行攻击行为，直到条件不满足时退回追逐状态。
// 依赖：构造函数注入委托，不直接持有 MonoBehaviour 引用。
// 转移条件（任一满足则退回 ChaseState）：
//   - hasTarget() 返回 false（目标已死亡/消失）
//   - shouldKeepAttacking() 返回 false（目标移出攻击范围）
// 攻击行为：每帧调用 onAttack 委托（触发伤害判定、播放特效等）。
using System;

public class AttackState : IState
{
    private readonly StateMachine stateMachine;
    private readonly Func<bool> hasTarget;
    private readonly Func<bool> shouldKeepAttacking;
    private readonly Func<IState> getChaseState;
    private readonly Action onAttack;

    public AttackState(
        StateMachine stateMachine,
        Func<bool> hasTarget,
        Func<bool> shouldKeepAttacking,
        Func<IState> getChaseState,
        Action onAttack = null)
    {
        this.stateMachine = stateMachine;
        this.hasTarget = hasTarget;
        this.shouldKeepAttacking = shouldKeepAttacking;
        this.getChaseState = getChaseState;
        this.onAttack = onAttack;
    }

    public void Enter()
    {
    }

    public void Update()
    {
        if (hasTarget == null || !hasTarget())
        {
            TransitionToChase();
            return;
        }

        if (shouldKeepAttacking != null && !shouldKeepAttacking())
        {
            TransitionToChase();
            return;
        }

        onAttack?.Invoke();
    }

    public void Exit()
    {
    }

    private void TransitionToChase()
    {
        IState chaseState = getChaseState?.Invoke();
        if (chaseState != null)
        {
            stateMachine.TransitionTo(chaseState);
        }
    }
}
