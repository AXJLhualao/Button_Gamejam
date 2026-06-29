using UnityEngine;

[RequireComponent(typeof(SoldierCombat))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CombatStats))]
[RequireComponent(typeof(TeamMember))]
[RequireComponent(typeof(TargetFollowing))]
public class Soldier : Entity
{
    private IState moveToPathState;
    private IState patrolState;
    private int? patrolStartWaypoint;

    protected override void Awake()
    {
        base.Awake();
        targetFollowing?.SetEnableDetectionOnEnable(false);
        BuildDeathState();

        CombatStates states = BuildCombatStates(
            () => gameObject.SetActive(false),
            reversePatrol: true,
            getStartWaypoint: () => patrolStartWaypoint);

        patrolState = states.Patrol;

        moveToPathState = WithAnimation(
            new MoveToPathState(
                stateMachine,
                transform,
                () => MoveSpeed,
                () => patrolState,
                OnArrivedPath,
                FaceMoveDirection,
                true),
            patrol_animation);
    }

    private void OnEnable()
    {
        SetCurrentPath(null);
        patrolStartWaypoint = null;
        Debug.Log(targetFollowing);
        targetFollowing?.SetDetectionActive(false);
        if (stateMachine != null && moveToPathState != null)
            stateMachine.Initialize(moveToPathState);
    }

    private void OnArrivedPath(Path path, int startWaypoint)
    {
        SetCurrentPath(path);
        patrolStartWaypoint = startWaypoint;
        targetFollowing?.SetDetectionActive(true);
    }
}
