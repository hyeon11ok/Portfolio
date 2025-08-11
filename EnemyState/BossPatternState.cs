using UnityEngine;

public class BossPatternState:EnemyBaseState
{
    private BaseCondition playerCondition;

    private float patternDelay = 0.5f; // 패턴 딜레이
    private float patternTimer = 0f; // 패턴 타이머

    public BossPatternState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void StateEnter()
    {
        SetAnimationClip();

        if(playerCondition == null)
            playerCondition = stateMachine.Player.GetComponent<PlayerController>().Condition;

        moveSpeedModifier = 0f; // 공격 상태에서는 이동하지 않음
        stateMachine.Enemy.NavMeshAgent.isStopped = true; // NavMeshAgent를 정지시킴
        stateMachine.Enemy._Rigidbody.velocity = Vector3.zero; // Rigidbody를 정지시킴
        base.StateEnter();

        patternTimer = Time.time;

        StartAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);
        stateMachine.Enemy._Animator.SetTrigger(stateMachine.Enemy.AnimationData.AttackParameterHash);
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(stateMachine.Enemy.AnimationData.IdleParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        //if(Time.time - patternTimer < patternDelay)
        //{
        //    // 애니메이션 정지
        //    stateMachine.Enemy._Animator.speed = 0.1f; // 애니메이션 속도를 0으로 설정하여 정지
        //    return;
        //}

        // 애니메이션 실행
        //stateMachine.Enemy._Animator.speed = 1f; // 애니메이션 속도를 원래대로 설정

        if(stateMachine.Enemy._Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            stateMachine.Enemy._Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.85f)
        {
            if(stateMachine.ChangeState(EnemyStateType.Idle))
                return;
        }
    }

    void SetAnimationClip()
    {
        AnimationClip newClip = stateMachine.Enemy.GetPatternAnimationClip();

        AnimatorOverrideController overrideController = new AnimatorOverrideController(stateMachine.Enemy._Animator.runtimeAnimatorController);
        overrideController["EmptyAnim"] = newClip;
        stateMachine.Enemy._Animator.runtimeAnimatorController = overrideController;
    }
}
