// 2026.6.29 --v1 
// 动画状态装饰器 —— 为任意 IState 注入动画播放逻辑。
// 设计模式：装饰器（Decorator）
//   - 包裹一个已有状态（如 PatrolState），在不修改原状态代码的前提下
//     在 Enter 时播放指定动画
//   - Update / Exit 完全委托给被装饰状态
// 用法：
//   IState animatedPatrol = new AnimatedState(patrolState, "Walk", animator.Play);
//   stateMachine.TransitionTo(animatedPatrol);
using System;

public class AnimatedState : IState
{
    private readonly IState state;
    private readonly string animationStateName;
    private readonly Action<string> playAnimation;

    public AnimatedState(IState state, string animationStateName, Action<string> playAnimation)
    {
        this.state = state;
        this.animationStateName = animationStateName;
        this.playAnimation = playAnimation;
    }

    public void Enter()
    {
        if (!string.IsNullOrEmpty(animationStateName))
        {
            playAnimation?.Invoke(animationStateName);
        }

        state.Enter();
    }

    public void Update()
    {
        state.Update();
    }

    public void Exit()
    {
        state.Exit();
    }
}
