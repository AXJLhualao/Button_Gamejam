using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private float movespeed = 3;
    [SerializeField] private Path current_path;
    [SerializeField] private TargetFollowing target_following;
    [Header("动画状态名称")]
    [SerializeField] private string patrol_animation = "Patrol";
    [SerializeField] private string chase_animation = "Chase";
    [SerializeField] private string attack_animation = "Attack";
    private IState patrolState;
    private IState chaseState;
    private IState attackState;

    /// <summary>
    /// 初始化 Enemy 的路径归属、巡逻、追击和攻击状态。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (current_path == null)
        {
            GameObject pathObject = GameObject.Find("GamePath");
            if (pathObject != null)
            {
                current_path = pathObject.GetComponent<Path>();
            }
        }

        SetCurrentPath(current_path);

        attackState = WithAnimation(
            new AttackState(
                stateMachine,
                () => target_following != null && target_following.HasTarget(),
                () => target_following != null && target_following.IsCloseToTarget(),
                () => chaseState),
            attack_animation);

        chaseState = WithAnimation(
            new ChaseState(
                stateMachine,
                target_following,
                () => patrolState,
                () => movespeed,
                () => attackState,
                () => target_following != null && target_following.IsCloseTarget,
                FaceMoveDirection),
            chase_animation); // 定义追逐状态，并传入获取巡逻/攻击状态的委托

        patrolState = WithAnimation(
            new PatrolState( // 定义巡逻状态，并传入获取追逐状态的委托
                stateMachine,
                transform,
                current_path,
                target_following,
                () => chaseState,
                () => movespeed,
                () => gameObject.SetActive(false),
                FaceMoveDirection),
            patrol_animation);
    }

    /// <summary>
    /// 每次启用时从路径起点重新开始巡逻。
    /// </summary>
    private void OnEnable()
    {
        SetCurrentPath(current_path);
        stateMachine.Initialize(patrolState);
    }
}
