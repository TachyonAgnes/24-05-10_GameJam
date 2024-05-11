using UnityEngine;

public class AttackTargetNode : ActionNode
{
    private EnemyBT enemyBT;
    private Transform target;
    private float lastExecutedTime = 0f;
    private float executionInterval;
    private float attackingAngle;
    private bool isFirstExecution = true;

    public AttackTargetNode(EnemyBT enemyBT, float attackingAngle, float executionInterval)
    {
        this.enemyBT = enemyBT;
        this.executionInterval = executionInterval;
        this.attackingAngle = attackingAngle;
    }
    public override NodeStatus Execute() {
        if (enemyBT.target == null)
        {
            return NodeStatus.FAILURE;
        }
        target = enemyBT.target;

        Vector3 directionToTarget = target.position - enemyBT.transform.position;

        float angle = Vector3.Angle(enemyBT.transform.forward, directionToTarget);
        if (angle < attackingAngle / 2)
        {
            if (isFirstExecution)
            {
                isFirstExecution = false;
                Attack();
            }
            if (Time.time - lastExecutedTime > executionInterval)
            {
                Attack();
                lastExecutedTime = Time.time;
            }
        }

        return NodeStatus.SUCCESS;
    }

    private void Attack() {
        Debug.Log("DADADADADADA");
    }
}
