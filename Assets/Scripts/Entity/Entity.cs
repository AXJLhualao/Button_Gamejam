using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected Transform flip_root;
    [SerializeField] private Path currentPath;
    protected StateMachine stateMachine;
    public Path CurrentPath => currentPath;

    /// <summary>
    /// 初始化通用状态机、动画器和翻转根节点。
    /// </summary>
    protected virtual void Awake()
    {
        stateMachine = new StateMachine();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (flip_root == null)
        {
            flip_root = animator != null ? animator.transform : transform;
        }
    }

    /// <summary>
    /// 每帧驱动当前状态更新。
    /// </summary>
    protected virtual void Update()
    {
        stateMachine?.Update();
    }

    /// <summary>
    /// 设置实体当前所属路径，用于目标识别时判断是否同路。
    /// </summary>
    public void SetCurrentPath(Path path)
    {
        currentPath = path;
    }

    /// <summary>
    /// 判断另一个实体是否与当前实体处于同一个路径。
    /// </summary>
    public bool IsOnSamePath(Entity other)
    {
        return other != null && currentPath != null && other.CurrentPath == currentPath;
    }

    /// <summary>
    /// 为状态追加进入时播放动画的行为。
    /// </summary>
    protected IState WithAnimation(IState state, string animationStateName)
    {
        return new AnimatedState(state, animationStateName, PlayAnimation);
    }

    /// <summary>
    /// 播放 Animator Controller 中指定名称的状态。
    /// </summary>
    private void PlayAnimation(string animationStateName)
    {
        if (animator == null) return;

        animator.Play(animationStateName);
    }

    /// <summary>
    /// 根据水平移动方向翻转显示根节点，默认美术朝向右侧。
    /// </summary>
    protected void FaceMoveDirection(Vector3 moveDirection)
    {
        if (flip_root == null || Mathf.Abs(moveDirection.x) < 0.01f) return;

        Vector3 scale = flip_root.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(moveDirection.x);
        flip_root.localScale = scale;
    }
}
