using UnityEngine;

public class Solider : Entity
{
    [SerializeField] private float movespeed = 3;
    [SerializeField] private Path current_path;
    [SerializeField] private TargetFollowing target_following;
    [Header("动画状态名称")]
    [SerializeField] private string patrol_animation = "Patrol";
    [SerializeField] private string chase_animation = "Chase";
    [SerializeField] private string attack_animation = "Attack";
    private IState moveToPathState;
    private IState patrolState;
    private IState chaseState;
    private IState attackState;
    private int? patrolStartWaypoint;

    /// <summary>
    /// 初始化 Soldier 的上路、反向巡逻、追击和攻击状态。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (target_following == null)
        {
            target_following = GetComponent<TargetFollowing>();
        }

        target_following?.SetEnableDetectionOnEnable(false);

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
                null,
                () => movespeed,
                () => attackState,
                () => target_following != null && target_following.IsCloseTarget,
                FaceMoveDirection),
            chase_animation);

        patrolState = WithAnimation(
            new PatrolState(
                stateMachine,
                transform,
                () => current_path,
                target_following,
                () => chaseState,
                () => movespeed,
                () => gameObject.SetActive(false),
                FaceMoveDirection,
                true,
                () => patrolStartWaypoint),
            patrol_animation);

        moveToPathState = WithAnimation(
            new MoveToPathState(
                stateMachine,
                transform,
                () => movespeed,
                () => patrolState,
                OnArrivedPath,
                FaceMoveDirection,
                true),
            patrol_animation);
    }

    /// <summary>
    /// 每次启用时先进入上路状态，并在到达路径前关闭目标检测。
    /// </summary>
    private void OnEnable()
    {
        current_path = null;
        patrolStartWaypoint = null;
        SetCurrentPath(null);
        target_following?.SetDetectionActive(false);
        stateMachine.Initialize(moveToPathState);
    }

    /// <summary>
    /// Soldier 到达最近路径后记录路径归属和反向巡逻起点。
    /// </summary>
    private void OnArrivedPath(Path path, int startWaypoint)
    {
        current_path = path;
        patrolStartWaypoint = startWaypoint;
        SetCurrentPath(path);
        target_following?.SetDetectionActive(true);
    }
}
