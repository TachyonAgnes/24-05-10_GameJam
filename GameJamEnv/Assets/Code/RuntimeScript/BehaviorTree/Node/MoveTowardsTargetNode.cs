using UnityEngine;

public class MoveTowardsTargetNode : ActionNode
{
    private EnemyBT enemyBT;
    private Rigidbody rb;
    private float moveSpeed;

    public MoveTowardsTargetNode(EnemyBT enemyBT, float moveSpeed)
    {
        this.enemyBT = enemyBT;
        this.rb = enemyBT.GetComponent<Rigidbody>();
        this.moveSpeed = moveSpeed;
    }

    public override NodeStatus Execute() 
    {
        if (enemyBT.target != null)
        {
            MoveTowardsTarget();
            return NodeStatus.SUCCESS;
        }else{
            return NodeStatus.FAILURE;
        }
    }

    public void MoveTowardsTarget()
    {
        Vector3 targetPosition = enemyBT.target.position;
        Vector3 direction = (targetPosition - enemyBT.transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }
}
