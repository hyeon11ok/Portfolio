using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossController : EnemyController
{
    [SerializeField] private List<BossPattern> patterns;
    private int currentPatternIndex = 0;

    protected override void Initialize()
    {
        base.Initialize();

        for(int i = 0; i < patterns.Count; i++)
        {
            patterns[i].Init(Condition, _CombatController);
        }   
    }

    public void SetRandomPattern()
    {
        currentPatternIndex = UnityEngine.Random.Range(0, patterns.Count);
    }

    public override AnimationClip GetPatternAnimationClip()
    {
        SetRandomPattern();

        if(currentPatternIndex < 0 || currentPatternIndex >= patterns.Count)
        {
            Debug.LogError("Invalid pattern index: " + currentPatternIndex);
            return null;
        }

        return patterns[currentPatternIndex].AnimationClip;
    }

    public override void AttackAction()
    {
        patterns[currentPatternIndex].PatternAction();
    }

    public override void Die()
    {
        base.Die();
    }

    protected override void SetEnemyState()
    {
        StateMachine.AddState(EnemyStateType.Idle, new BossIdleState(StateMachine));
        StateMachine.AddState(EnemyStateType.Chase, new BossChaseState(StateMachine));
        StateMachine.AddState(EnemyStateType.Dash, new BossDashState(StateMachine));
        StateMachine.AddState(EnemyStateType.Attack, new BossPatternState(StateMachine));
        StateMachine.AddState(EnemyStateType.Die, new BossDownState(StateMachine));

        StateMachine.StartStateMachine(EnemyStateType.Idle);
    }
}
