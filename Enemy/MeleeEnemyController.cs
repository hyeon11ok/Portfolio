using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    /// <summary>
    /// 근거리 적의 공격 행동
    /// 적 충돌 체크 및 공격
    /// </summary>
    public override void AttackAction()
    {
        Collider[] hitColliders = _CombatController.GetTargetColliders(LayerMask.GetMask("Player"));

        float attackPower = Condition.GetTotalCurrentValue(ConditionType.AttackPower);
        float criticalChance = Condition.GetTotalCurrentValue(ConditionType.CriticalChance);
        float criticalDamage = Condition.GetTotalCurrentValue(ConditionType.CriticalDamage);

        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider.TryGetComponent(out IDamagable player))
            {
                // 크리티컬 판정
                bool isCritical = Random.value < criticalChance;

                float finalDamage = attackPower;
                if(isCritical)
                {
                    finalDamage *= criticalDamage;
                }

                // 플레이어에게 피해를 입히는 로직
                if(!player.GetDamaged(finalDamage))
                {
                    StateMachine.ChangeState(EnemyStateType.Idle);
                }
                else
                {
                    DamageType damageType = isCritical ? DamageType.Critical : DamageType.Normal;
                    PoolingDamageUI damageUI = PoolManager.Instance.GetObject(PoolType.DamageUI).GetComponent<PoolingDamageUI>();
                    damageUI.InitDamageText(player.GetDamagedPos(), damageType, finalDamage);
                }
            }
        }
    }


    public override AnimationClip GetPatternAnimationClip()
    {
        throw new System.NotImplementedException();
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
}
