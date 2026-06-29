using UnityEngine;

public class CombatAnimationEvents : MonoBehaviour
{
    private CombatEntity combatEntity;

    private void Awake()
    {
        combatEntity = GetComponentInParent<CombatEntity>();
    }

    public void OnAttackHit()
    {
        combatEntity?.PerformAttack();
    }
}
