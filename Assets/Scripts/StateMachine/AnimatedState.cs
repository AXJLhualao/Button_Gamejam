/// <summary>
/// 动画状态装饰器 —— 为任意 IState 注入动画播放逻辑。
/// 用于在进入状态时播放动画，并在 Update 中检查动画是否播放完毕以决定是否重播。
/// </summary>
using System;
using UnityEngine;

public class AnimatedState : IState
{
    private readonly IState state;
    private readonly string animationStateName;
    private readonly Action<string> playAnimation;
    private readonly Func<Animator> getAnimator;
    private readonly bool replayWhenFinished;

    public AnimatedState(
        IState state,
        string animationStateName,
        Action<string> playAnimation,
        Func<Animator> getAnimator = null,
        bool replayWhenFinished = false)
    {
        this.state = state;
        this.animationStateName = animationStateName;
        this.playAnimation = playAnimation;
        this.getAnimator = getAnimator;
        this.replayWhenFinished = replayWhenFinished;
    }

    /// <summary>
    /// 进入状态时播放对应动画。
    /// </summary>
    public void Enter()
    {
        if (!string.IsNullOrEmpty(animationStateName))
        {
            playAnimation?.Invoke(animationStateName);
        }

        state.Enter();
    }

    /// <summary>
    /// 驱动内部状态，并在攻击动画结束时重播以循环出刀。
    /// </summary>
    public void Update()
    {
        state.Update();
        TryReplayAnimation();
    }

    /// <summary>
    /// 退出状态时委托给内部状态。
    /// </summary>
    public void Exit()
    {
        state.Exit();
    }

    /// <summary>
    /// 非 Loop 的 Attack 片段播放完后从头重播，以便反复触发命中事件。
    /// </summary>
    private void TryReplayAnimation()
    {
        if (!replayWhenFinished || string.IsNullOrEmpty(animationStateName))
        {
            return;
        }

        Animator animator = getAnimator();
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName(animationStateName)) return;
        if (stateInfo.normalizedTime < 1f) return;

        playAnimation?.Invoke(animationStateName);
    }
}
