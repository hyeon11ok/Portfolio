using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDashState : EnemyBaseState
{
    private float arrivedTime = 0.2f; // 대쉬 도착 시간
    private float timer = 0f;

    private Vector3 startPos;
    private Vector3 endPos;

    public BossDashState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        timer = 0;

        moveSpeedModifier = 0f;
        stateMachine.Enemy.NavMeshAgent.enabled = false;
        stateMachine.Enemy._Rigidbody.isKinematic = true;

        Vector3 dir = (stateMachine.Player.transform.position - stateMachine.Enemy.transform.position).normalized;
        dir.y = 0; // y축은 현재 위치 유지

        startPos = stateMachine.Enemy.transform.position;
        endPos = stateMachine.Player.transform.position - dir * stateMachine.Enemy.Condition.GetTotalCurrentValue(ConditionType.AttackRange);

        StartAnimation(stateMachine.Enemy.AnimationData.ChaseParameterHash);
        //stateMachine.Enemy._Animator.speed = 0f;
    }


    public override void StateUpdate()
    {
        base.StateUpdate();

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / arrivedTime);
        stateMachine.Enemy.transform.position = Vector3.Lerp(startPos, endPos, t);

        if(t >= 1f)
        {
            // 대쉬가 끝나면 Idle 상태로 전환
            if(stateMachine.ChangeState(EnemyStateType.Attack))
                return;
        }
    }

    public override void StateExit()
    {
        stateMachine.Enemy.NavMeshAgent.enabled = true;
        stateMachine.Enemy._Rigidbody.isKinematic = false;
        base.StateExit();
        StopAnimation(stateMachine.Enemy.AnimationData.ChaseParameterHash);
        //stateMachine.Enemy._Animator.speed = 1f;
    }
}
