public class EnemyChaseState : EnemyMoveState
{
    public EnemyChaseState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void StateEnter()
    {
        moveSpeedModifier = 1.5f;
        stateMachine.Enemy.NavMeshAgent.isStopped = false; 
        base.StateEnter();

        targetPos = stateMachine.Player.transform.position;
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

        if(!stateMachine.Enemy.IsInRange(ConditionType.ChaseRange))
        {
            // IdleState로 변환
            stateMachine.Enemy.SetPatrolPivot();
            if(stateMachine.ChangeState(EnemyStateType.Idle))
                return;
        }
        if(stateMachine.HasState(EnemyStateType.Attack) && stateMachine.Enemy.IsInRange(ConditionType.AttackRange))
        {
            // AttackState로 변환
            if(stateMachine.ChangeState(EnemyStateType.Attack))
                return;
        }
    }

    public override void StatePhysicsUpdate()
    {
        targetPos = stateMachine.Player.transform.position;
        base.StatePhysicsUpdate();
    }
}
