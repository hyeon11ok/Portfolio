using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChaseState : EnemyMoveState
{
    float foundRange;

    public BossChaseState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void StateEnter()
    {
        moveSpeedModifier = 1f;
        stateMachine.Enemy.NavMeshAgent.isStopped = false;
        base.StateEnter();

        targetPos = stateMachine.Player.transform.position;
        foundRange = GetDistanceToTarget();
        StartAnimation(stateMachine.Enemy.AnimationData.ChaseParameterHash);
    }

    public override void StateExit()
    {
        base.StateExit();

        StopAnimation(stateMachine.Enemy.AnimationData.ChaseParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(foundRange > stateMachine.Enemy.Condition.GetTotalMaxValue(ConditionType.ChaseRange))
        {
            if(GetDistanceToTarget() <= stateMachine.Enemy.Condition.GetTotalMaxValue(ConditionType.ChaseRange))
            {
                if(stateMachine.ChangeState(EnemyStateType.Dash))
                    return;
            }
        }
        else
        {
            if(stateMachine.Enemy.IsInRange(ConditionType.AttackRange))
            {
                // AttackState로 변환
                if(stateMachine.ChangeState(EnemyStateType.Attack))
                    return;
            }
        }
    }

    public override void StatePhysicsUpdate()
    {
        targetPos = stateMachine.Player.transform.position;
        base.StatePhysicsUpdate();
    }
}
