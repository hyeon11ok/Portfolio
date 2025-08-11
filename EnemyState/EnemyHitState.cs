using UnityEngine;

public class EnemyHitState : EnemyBaseState
{
    public EnemyHitState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void StateEnter()
    {
        moveSpeedModifier = 0f; // 공격 상태에서는 이동하지 않음
        stateMachine.Enemy.NavMeshAgent.isStopped = true; // NavMeshAgent를 정지시킴
        stateMachine.Enemy._Rigidbody.velocity = Vector3.zero; // Rigidbody를 정지시킴
        StartAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);
        stateMachine.Enemy._Animator.SetTrigger(stateMachine.Enemy.AnimationData.HitParameterHash);
        base.StateEnter();
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(stateMachine.Enemy._Animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") &&
            stateMachine.Enemy._Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
        {
            // 추적 범위를 벗어남
            if(!stateMachine.Enemy.IsInRange(ConditionType.ChaseRange))
            {
                // AttackState로 변환
                if(stateMachine.ChangeState(EnemyStateType.Idle))
                    return;
            }
            // 추적 범위는 벗어나지 않았고 공격 범위를 벗어남
            else if(!stateMachine.Enemy.IsInRange(ConditionType.AttackRange))
            {
                // AttackState로 변환
                if(stateMachine.ChangeState(EnemyStateType.Chase))
                    return;
            }
            else
            {
                // Hit 상태가 끝나면 다시 Attack 상태로 전환
                if(stateMachine.ChangeState(EnemyStateType.Attack))
                    return;
            }
        }
    }
}
