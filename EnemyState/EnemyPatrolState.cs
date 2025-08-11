using UnityEngine;

public class EnemyPatrolState : EnemyMoveState
{
    private float startTime;
    private float minimumPatrolTime = 3f; // 최소 순찰 시간

    public EnemyPatrolState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void StateEnter()
    {
        moveSpeedModifier = 1f;
        base.StateEnter();

        targetPos = stateMachine.PatrolPoint;
        StartAnimation(stateMachine.Enemy.AnimationData.PatrolParameterHash);
        startTime = Time.time;
    }

    public override void StateExit()
    {
        base.StateExit();

        StopAnimation(stateMachine.Enemy.AnimationData.PatrolParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(stateMachine.Enemy.IsInRange(ConditionType.ChaseRange))
        {
            // ChaseState로 전환
            if(stateMachine.ChangeState(EnemyStateType.Chase))
                return;
        }

        // 목표 지점에 도착한 경우
        if(IsArrivePatrolPoint())
        {
            // IdleState로 전환
            if(stateMachine.ChangeState(EnemyStateType.Idle))
                return;
        }
    }

    public override void StatePhysicsUpdate()
    {
        targetPos = stateMachine.PatrolPoint;
        base.StatePhysicsUpdate();
    }

    private bool IsArrivePatrolPoint()
    {
        // 목표 지점까지의 거리가 0.2 이하인 경우 도착한 것으로 간주
        if(GetDistanceToTarget() <= 0.2f)
        {
            return true;
        }
        
        // 2D 뷰 모드에서 이동 속도가 1 이하인 경우
        // 최소 순찰 시간 이후에 도착한 것으로 간주
        if(viewMode == ViewModeType.View2D && stateMachine.Enemy._Rigidbody.velocity.magnitude < 0.5f && Time.time - startTime > minimumPatrolTime)
        {
            return true;
        }

        return false;
    }
}
