using UnityEngine;

public class TeamMember : MonoBehaviour
{
    [SerializeField] private Team team;
    public Team Team => team;
}
