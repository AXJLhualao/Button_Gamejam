using System;
using UnityEngine;

/// <summary>
/// 死亡状态：禁用碰撞与行为组件，延迟后销毁对象。
/// </summary>
public class DeathState : IState
{
    private readonly MonoBehaviour owner;
    private readonly Action onEnter;
    private readonly float destroyDelay;
    private float elapsed;

    public DeathState(MonoBehaviour owner, Action onEnter = null, float destroyDelay = 1f)
    {
        this.owner = owner;
        this.onEnter = onEnter;
        this.destroyDelay = destroyDelay;
    }

    /// <summary>
    /// 进入死亡状态时执行清理回调。
    /// </summary>
    public void Enter()
    {
        elapsed = 0f;
        onEnter?.Invoke();
    }

    /// <summary>
    /// 等待销毁延迟结束后销毁对象。
    /// </summary>
    public void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= destroyDelay)
        {
            UnityEngine.Object.Destroy(owner.gameObject);
        }
    }

    /// <summary>
    /// 退出死亡状态时无需额外处理。
    /// </summary>
    public void Exit()
    {
    }
}
