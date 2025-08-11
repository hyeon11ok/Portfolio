using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleState:EnemyBaseState
{
    private float waitingStartTime;
    private float waitingEndTime = 2f;
    Vector3 nextPoint;
    bool isWanderLocationValid = false;

    public EnemyIdleState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void StateEnter()
    {
        moveSpeedModifier = 0; // Idle 상태에서는 이동 속도를 0으로 설정
        stateMachine.Enemy.NavMeshAgent.isStopped = true; // NavMeshAgent를 정지시킴
        stateMachine.Enemy._Rigidbody.velocity = Vector3.zero; // Rigidbody를 정지시킴
        base.StateEnter();

        StartAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);

        if(stateMachine.HasState(EnemyStateType.Patrol))
        {
            SetIdleState();
        }
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(stateMachine.Enemy.IsInRange(ConditionType.AttackRange))
        {
            // AttackState로 변환
            if(stateMachine.ChangeState(EnemyStateType.Attack))
                return;
        }

        if(stateMachine.Enemy.IsInRange(ConditionType.ChaseRange))
        {
            // MoveState로 변환
            if(stateMachine.ChangeState(EnemyStateType.Chase))
                return;
        }

        if(stateMachine.HasState(EnemyStateType.Patrol))
        {
            if(waitingEndTime <= Time.time - waitingStartTime)
            {
                if(!isWanderLocationValid)
                {
                    SetIdleState();
                    return; // 유효한 위치를 찾지 못했을 경우 대기 상태 유지
                }

                // 대기 시간이 끝나면 Target을 다음 PatrolPoint로 설정 후 MoveState로 전환
                stateMachine.SetPatrolPoint(nextPoint);
                if(stateMachine.ChangeState(EnemyStateType.Patrol))
                    return;
            }
        }
    }

    private void SetIdleState()
    {
        waitingStartTime = Time.time;
        nextPoint = GetWanderLocation();
    }

    /// <summary>
    /// 탐색 지점 검출 메서드
    /// </summary>
    /// <returns></returns>
    private Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        // 순찰 반경 내에서 랜덤한 위치를 탐색, 유효한 위치, 경로인지 확인
        // 탐색한 지점의 거리가 patrolRadius의 30% 이상인 경우에만 유효한 위치로 간주
        // 최대 30번 시도하여 유효한 위치를 찾습니다.
        for(int i = 0; i < 30; i++)
        {
            float patrolRange = stateMachine.Enemy.Condition.GetTotalCurrentValue(ConditionType.PatrolRange);

            Vector2 samplePosV2 = Random.insideUnitCircle * patrolRange;
            Vector3 sample = new Vector3(samplePosV2.x, 0, samplePosV2.y);

            if(NavMesh.SamplePosition(stateMachine.Enemy.patrolPivot + sample, out hit, patrolRange, NavMesh.AllAreas))
            {
                if(Mathf.Abs(stateMachine.Enemy.transform.position.y - hit.position.y) > 1f)
                {
                    continue; // 높이 차이가 너무 큰 경우 무시
                }

                if(Vector3.Distance(stateMachine.Enemy.transform.position, hit.position) > patrolRange * 0.8f)
                {
                    isWanderLocationValid = true; // 유효한 위치를 찾았음을 표시
                    return hit.position; // 유효한 위치를 반환
                }
            }
        }

        isWanderLocationValid = false; // 유효한 위치를 찾지 못했음을 표시
        return stateMachine.Enemy.patrolPivot; // Fallback to patrol pivot if no valid position found
    }
}
