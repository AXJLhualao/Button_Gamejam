using UnityEngine;

[RequireComponent(typeof(EnemyCombat))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CombatStats))]
[RequireComponent(typeof(TeamMember))]
public class Enemy : Entity
{
    private IState patrolState;

    protected override void Awake()
    {
        base.Awake();

        GameObject pathObject = GameObject.Find("GamePath");
        currentPath = pathObject.GetComponent<Path>();

        BuildDeathState();

        CombatStates states = BuildCombatStates(
            () => gameObject.SetActive(false));

        patrolState = states.Patrol;
    }

    private void OnEnable()
    {
        stateMachine.Initialize(patrolState);
    }
}
