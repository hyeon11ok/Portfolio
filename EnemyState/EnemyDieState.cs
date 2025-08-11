using UnityEngine;

public class EnemyDieState:EnemyBaseState
{
    private ItemDropper itemDropper; // 아이템 드랍을 위한 ItemDropper 컴포넌트

    public EnemyDieState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
        stateMachine.Enemy.TryGetComponent<ItemDropper>(out itemDropper);
    }

    public override void StateEnter()
    {

        moveSpeedModifier = 0; // Idle 상태에서는 이동 속도를 0으로 설정
        stateMachine.Enemy.NavMeshAgent.enabled = false; // NavMeshAgent를 비활성화하여 이동을 중지시킴
        stateMachine.Enemy._Rigidbody.isKinematic = true; // Rigidbody를 Kinematic으로 설정하여 물리적 상호작용을 비활성화
        stateMachine.Enemy.GetComponent<Collider>().enabled = false; // Collider를 비활성화하여 충돌을 방지
        SoundManager.Instance.PlaySFX(stateMachine.Enemy.transform.position, "Death");
        stateMachine.Enemy._Animator.SetTrigger(stateMachine.Enemy.AnimationData.DieParameterHash);
        base.StateEnter();

        // 아이템 드랍, 골드 획득 로직
        stateMachine.Player.GetComponent<PlayerController>()?.Condition.ChangeGold(stateMachine.Enemy.Condition.GetTotalCurrentValue(ConditionType.Gold));

    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        // 애니메이션 상태가 "Die"이고, 애니메이션이 거의 끝났을 때
        if(stateMachine.Enemy._Animator.GetCurrentAnimatorStateInfo(1).IsName("Die") && 
            stateMachine.Enemy._Animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.95f)
        {
            itemDropper?.TryDropItem();
            stateMachine.Enemy.room.CheckClear();
            stateMachine.Enemy.gameObject.SetActive(false); // 적 오브젝트를 비활성화
        }
    }
}