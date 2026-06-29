using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected Path currentPath;
    [SerializeField] protected TargetFollowing targetFollowing;
    [Header("动画状态名称")]
    [SerializeField] protected string patrol_animation = "Patrol";
    [SerializeField] protected string chase_animation = "Chase";
    [SerializeField] protected string attack_animation = "Attack";
    [SerializeField] protected string death_animation = "Death";
    protected StateMachine stateMachine = new StateMachine();
    protected IState deathState;
    protected CombatStats combatStats;

    public Path CurrentPath => currentPath;

    protected struct CombatStates
    {
        public IState Patrol;
        public IState Chase;
        public IState Attack;
    }

    protected virtual void Awake()
    {
        combatStats = GetComponent<CombatStats>();
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        stateMachine?.Update();
    }

    protected float MoveSpeed => combatStats.MoveSpeed;

    public void SetCurrentPath(Path path)
    {
        currentPath = path;
    }

    public bool IsOnSamePath(Entity other)
    {
        return other.CurrentPath == currentPath;
    }

    protected CombatStates BuildCombatStates(
        Action onPathComplete,
        bool reversePatrol = false,
        Func<int?> getStartWaypoint = null)
    {
        IState patrol = null;
        IState chase = null;
        IState attack = null;

        attack = WithAnimation(
            new AttackState(
                stateMachine,
                () => targetFollowing.HasTarget(),
                () => targetFollowing.IsCloseToTarget(),
                () => chase),
            attack_animation,
            true);

        chase = WithAnimation(
            new ChaseState(
                stateMachine,
                targetFollowing,
                () => patrol,
                () => MoveSpeed,
                () => attack,
                () => targetFollowing.IsCloseToTarget(),
                FaceMoveDirection),
            chase_animation);

        patrol = WithAnimation(
            new PatrolState(
                stateMachine,
                transform,
                () => currentPath,
                targetFollowing,
                () => chase,
                () => MoveSpeed,
                onPathComplete,
                FaceMoveDirection,
                reversePatrol,
                getStartWaypoint),
            patrol_animation);

        return new CombatStates { Patrol = patrol, Chase = chase, Attack = attack };
    }

    protected virtual void BuildDeathState(float destroyDelay = 1f)
    {
        deathState = WithAnimation(
            new DeathState(this, null, destroyDelay),
            death_animation);
    }

    public void EnterDeathState(Action onEnter = null)
    {
        onEnter?.Invoke();
        stateMachine.TransitionTo(deathState);
    }

    protected IState WithAnimation(IState state, string animationStateName, bool replayWhenFinished = false)
    {
        if (string.IsNullOrEmpty(animationStateName))
            return state;

        return new AnimatedState(state, animationStateName, PlayAnimation, () => animator, replayWhenFinished);
    }

    private void PlayAnimation(string animationStateName)
    {
        animator.Play(animationStateName);
    }

    protected void FaceMoveDirection(Vector3 moveDirection)
    {
        if (Mathf.Abs(moveDirection.x) < 0.01f) return;
        Vector3 scale = animator.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(moveDirection.x);
        animator.transform.localScale = scale;
    }
}
