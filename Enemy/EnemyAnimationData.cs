using UnityEngine;

public class EnemyAnimationData
{
    private string idleParameterName = "Idle";
    private string patrolParameterName = "Patrol";
    private string chaseParameterName = "Chase";
    private string attackParameterName = "Attack";
    private string dieParameterName = "Die";
    private string hitParameterName = "Hit";

    public int IdleParameterHash { get; private set; }
    public int PatrolParameterHash { get; private set; }
    public int ChaseParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }
    public int DieParameterHash { get; private set; }
    public int HitParameterHash { get; private set; }

    public EnemyAnimationData()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        PatrolParameterHash = Animator.StringToHash(patrolParameterName);
        ChaseParameterHash = Animator.StringToHash(chaseParameterName);
        AttackParameterHash = Animator.StringToHash(attackParameterName);
        DieParameterHash = Animator.StringToHash(dieParameterName);
        HitParameterHash = Animator.StringToHash(hitParameterName);
    }
}
