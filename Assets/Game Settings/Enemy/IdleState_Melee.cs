using UnityEngine;

public class IdleState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    public IdleState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemyBase.idleTime;
    }
    public override void Update()
    {
        base.Update();
        if (enemy.PlayerInAgressionRange()) {
            stateMachine.ChangeState(enemy.recoveryState);
            return;
        }
        if (stateTimer < 0)
        {
            Debug.Log("I should change to move state");
            stateMachine.ChangeState(enemy.moveState);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}
