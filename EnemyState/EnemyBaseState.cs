using Unity.Services.Analytics.Internal;
using UnityEngine;

public class EnemyBaseState:IUnitState
{
    protected EnemyStateMachine stateMachine;
    protected ConditionData data;

    protected ViewModeType viewMode;

    protected float moveSpeedModifier = 1f;

    // StatePhysicsUpdate 메서드를 오버라이드하여 Base 이전에 초기화
    protected Vector3 targetPos;

    public EnemyBaseState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        data = stateMachine.Enemy.Condition.Data;
    }

    public virtual void StateEnter()
    {
        if(ViewManager.HasInstance)
        {
            viewMode = ViewManager.Instance.CurrentViewMode;
            ViewManager.Instance.OnViewChanged += SwitchView;
        }
        else
        {
            viewMode = ViewModeType.View2D; // 기본값 설정
        }
    }

    public virtual void StateExit()
    {
        if(ViewManager.HasInstance)
        {
            ViewManager.Instance.OnViewChanged -= SwitchView;
        }
    }

    public virtual void StatePhysicsUpdate()
    {
    }

    public virtual void StateUpdate()
    {
    }
    protected void StartAnimation(int animationHash)
    {
        stateMachine.Enemy._Animator.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        stateMachine.Enemy._Animator.SetBool(animationHash, false);
    }

    /// <summary>
    /// 2D/3D 시점 변환시 호출되는 메서드
    /// </summary>
    /// <param name="mode"></param>
    public void SwitchView(ViewModeType mode)
    {
        if(viewMode == mode) return;
        viewMode = mode;
    }

    protected Vector3 GetMovementDirection(Vector3 target)
    {
        Vector3 dir = target - stateMachine.Enemy.transform.position;

        // 2D인 경우 x축 y축만 고려
        if(viewMode == ViewModeType.View2D)
        {
            dir.z = 0;
        }
        dir.y = 0; // y축은 현재 위치 유지

        dir.Normalize();
        return dir;
    }

    protected void Move(Vector3 movementDirection)
    {
        float movementSpeed = GetMovementSpeed();
        float moveStop = 1;

        if(GetDistanceToTarget() < 0.2f)
        {
            moveStop = 0f;
            stateMachine.Enemy.NavMeshAgent.acceleration = 0f; // NavMeshAgent의 가속도를 0으로 설정하여 정지 상태 유지
        }
        else
        {
            moveStop = 1f;
        }

        // 2D인 경우 Rigidbody를 사용하고, 3D인 경우 NavMeshAgent를 사용
        if(viewMode == ViewModeType.View2D)
        {
            stateMachine.Enemy.NavMeshAgent.isStopped = true; // NavMeshAgent를 정지시킴
            stateMachine.Enemy._Rigidbody.velocity = movementDirection * movementSpeed * moveStop;
        }
        else
        {
            stateMachine.Enemy._Rigidbody.velocity = Vector3.zero; // Rigidbody를 정지시킴
            stateMachine.Enemy.NavMeshAgent.isStopped = false;
            stateMachine.Enemy.NavMeshAgent.speed = movementSpeed * moveStop;
            stateMachine.Enemy.NavMeshAgent.SetDestination(targetPos);
        }
    }

    protected float GetMovementSpeed()
    {
        float movementSpeed = stateMachine.MovementSpeed * moveSpeedModifier;
        return movementSpeed;
    }

    protected void Rotate(Vector3 movementDirection)
    {
        if(movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

            if(viewMode == ViewModeType.View2D)
            {
                stateMachine.Enemy.VisualTransform.rotation = targetRotation;
            }
            else
            {
                stateMachine.Enemy.VisualTransform.rotation = Quaternion.Lerp(stateMachine.Enemy.VisualTransform.rotation, targetRotation, stateMachine.Enemy.VisualRotateSpeed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// 현재 타겟까지의 거리를 반환하는 메서드
    /// </summary>
    /// <returns></returns>
    protected float GetDistanceToTarget()
    {
        if(viewMode == ViewModeType.View2D)
        {
            // 2D인 경우 x축과 y축만 고려하여 거리 계산
            Vector3 targetPos2D = new Vector3(targetPos.x, targetPos.y, stateMachine.Enemy.transform.position.z);
            return Vector3.Distance(stateMachine.Enemy.transform.position, targetPos2D);
        }

        return Vector3.Distance(stateMachine.Enemy.transform.position, targetPos);
    }
}
