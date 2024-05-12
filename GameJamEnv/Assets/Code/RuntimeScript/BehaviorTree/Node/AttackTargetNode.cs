using UnityEngine;
using System;

public class AttackTargetNode : ActionNode
{
    public event Action OnEnemyAttack;


    private EnemyBT enemyBT;
    private Transform target;
    private float lastExecutedTime = 0f;
    private float executionInterval;
    private float attackingAngle;
    private float attackingRange;
    

    public AttackTargetNode(EnemyBT enemyBT, float attackingAngle, float executionInterval, float attackingRange)
    {
        this.enemyBT = enemyBT;
        this.executionInterval = executionInterval;
        this.attackingAngle = attackingAngle;
        this.attackingRange = attackingRange;
    }
    public override NodeStatus Execute() {
        if (enemyBT.target == null)
        {
            return NodeStatus.FAILURE;
        }
        target = enemyBT.target;
        Vector3 directionToTarget = target.position - enemyBT.transform.position;
        float angle = Vector3.Angle(enemyBT.transform.forward, directionToTarget);
        float distance = Vector3.Distance(enemyBT.transform.position, target.position);
        if (angle < attackingAngle / 2 && distance <= attackingRange)
        {
            if (Time.time - lastExecutedTime >= executionInterval)
            {
                Attack();
                lastExecutedTime = Time.time;
            }
            return NodeStatus.SUCCESS; 
        }

        return NodeStatus.SUCCESS;
    }

    private void Attack() {
        Debug.Log("DADADA");
        OnEnemyAttack?.Invoke();
    }
}
