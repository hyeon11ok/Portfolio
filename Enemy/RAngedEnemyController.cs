using UnityEngine;

public class RangedEnemyController:EnemyController
{
    [Header("원거리 적 설정")]
    [SerializeField] private Transform projectileOffset;
    [Header("투사체 방식")]
    [SerializeField] private int projectileType;


    /// <summary>
    /// 원거리 적의 공격 행동
    /// 투사체 생성 및 발사
    /// </summary>
    public override void AttackAction()
    {
        FireProjectile(GameManager.Instance.Player.transform);
    }

    private void FireProjectile(Transform target)
    {
        Collider p_collider = target.GetComponent<Collider>();
        Vector3 direction = p_collider.bounds.center - projectileOffset.position;
        direction.Normalize();

        PoolType poolToUse = projectileType switch
        {
            0 => PoolType.projectile,
            1 => PoolType.ArrowProjectile,
            _ => PoolType.projectile
        };

        GameObject projectile = PoolManager.Instance.GetObject(poolToUse);
        projectile.transform.position = projectileOffset.position;

        // === 크리티컬 처리 ===
        float attackPower = Condition.GetTotalCurrentValue(ConditionType.AttackPower);
        float criticalChance = Condition.GetTotalCurrentValue(ConditionType.CriticalChance);
        float criticalDamage = Condition.GetTotalCurrentValue(ConditionType.CriticalDamage);

        bool isCritical = UnityEngine.Random.value < criticalChance;

        projectile.GetComponent<Projectile>()?.InitProjectile(
            dir: direction,
            speed: 10f,
            damage: attackPower,
            isCritical: isCritical,
            criticalDamageMultiplier: criticalDamage
        );
    }


    protected override void SetEnemyState()
    {
        StateMachine.AddState(EnemyStateType.Idle, new EnemyIdleState(StateMachine));
        StateMachine.AddState(EnemyStateType.Patrol, new EnemyPatrolState(StateMachine));
        StateMachine.AddState(EnemyStateType.Chase, new EnemyChaseState(StateMachine));
        StateMachine.AddState(EnemyStateType.Attack, new EnemyAttackState(StateMachine));
        StateMachine.AddState(EnemyStateType.Die, new EnemyDieState(StateMachine));
        StateMachine.AddState(EnemyStateType.Hit, new EnemyHitState(StateMachine));

        StateMachine.StartStateMachine(EnemyStateType.Idle);
    }

    public override AnimationClip GetPatternAnimationClip()
    {
        throw new System.NotImplementedException();
    }
}
