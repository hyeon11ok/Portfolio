using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleState :EnemyBaseState
{
    private float patternDelay = 2f; // 패턴 딜레이
    private float patternTimer = 0f; // 패턴 타이머

    public BossIdleState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void StateEnter()
    {
        moveSpeedModifier = 0; // Idle 상태에서는 이동 속도를 0으로 설정
        stateMachine.Enemy.NavMeshAgent.isStopped = true; // NavMeshAgent를 정지시킴
        stateMachine.Enemy._Rigidbody.velocity = Vector3.zero; // Rigidbody를 정지시킴
        int dieLayerIndex = stateMachine.Enemy._Animator.GetLayerIndex("DieLayer");
        stateMachine.Enemy._Animator.SetLayerWeight(dieLayerIndex, 0);
        base.StateEnter();

        stateMachine.Enemy._Animator.ResetTrigger(stateMachine.Enemy.AnimationData.DieParameterHash);
        StartAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);
        patternTimer = Time.time;
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Vector3 movementDirection = GetMovementDirection(stateMachine.Player.transform.position);

        Rotate(movementDirection);

        if(Time.time - patternTimer >= patternDelay)
        {
            // 패턴 딜레이가 끝나면 다음 패턴으로 전환
            if(stateMachine.Enemy.IsInRange(ConditionType.AttackRange))
            {
                // AttackState로 변환
                if(stateMachine.ChangeState(EnemyStateType.Attack))
                    return;
            }

            if(stateMachine.ChangeState(EnemyStateType.Chase))
                return;
        }
    }
}
