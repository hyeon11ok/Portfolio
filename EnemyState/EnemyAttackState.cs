using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    private BaseCondition playerCondition;

    float attackDelay;
    float startTime;

    public EnemyAttackState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
        
    }

    public override void StateEnter()
    {
        if(playerCondition == null)
            playerCondition = stateMachine.Player.GetComponent<PlayerController>().Condition;

        moveSpeedModifier = 0f; // 공격 상태에서는 이동하지 않음
        stateMachine.Enemy.NavMeshAgent.isStopped = true; // NavMeshAgent를 정지시킴
        stateMachine.Enemy._Rigidbody.velocity = Vector3.zero; // Rigidbody를 정지시킴

        float atkSpeed = stateMachine.Enemy.Condition.GetTotalCurrentValue(ConditionType.AttackSpeed);

        attackDelay = 1.0f / atkSpeed; // 공격 속도에 따라 딜레이 설정
        startTime = Time.time;
        base.StateEnter();

        StartAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);
        stateMachine.Enemy._Animator.SetTrigger(stateMachine.Enemy.AnimationData.AttackParameterHash);
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(stateMachine.Enemy._Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if(stateMachine.Enemy._Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
            {
                if(playerCondition.IsDied)
                {
                    // IdleState로 변환
                    if(stateMachine.ChangeState(EnemyStateType.Idle))
                        return;
                }

                // 추적 범위를 벗어남
                if(!stateMachine.Enemy.IsInRange(ConditionType.ChaseRange))
                {
                    // IdleState로 변환
                    if(stateMachine.ChangeState(EnemyStateType.Idle))
                        return;
                }

                // 추적 범위는 벗어나지 않았고 공격 범위를 벗어남
                if(!stateMachine.Enemy.IsInRange(ConditionType.AttackRange))
                {
                    // AttackState로 변환
                    if(stateMachine.ChangeState(EnemyStateType.Chase))
                        return;
                }
            }
        }
        else
        {
            Vector3 movementDirection = GetMovementDirection(stateMachine.Player.transform.position);

            Rotate(movementDirection);
        }

        if(Time.time - startTime < attackDelay)
        {
            // 공격 딜레이가 끝나지 않았으면 대기
            return;
        }

        // 공격 행동 수행
        stateMachine.Enemy._Animator.SetTrigger(stateMachine.Enemy.AnimationData.AttackParameterHash);
        startTime = Time.time; // 다음 공격을 위한 시간 초기화
    }
}
