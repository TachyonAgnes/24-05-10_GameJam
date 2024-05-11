using UnityEngine;

public class LookAtTargetNode : ActionNode
{
    private EnemyBT enemyBT;
    private float detectRadius;
    private float detectAngle;
    private float turnSpeed;

    public LookAtTargetNode(EnemyBT enemyBT, float detectRadius, float detectAngle, float turnSpeed)
    {
        this.enemyBT = enemyBT;
        this.detectRadius = detectRadius;
        this.detectAngle = detectAngle;
        this.turnSpeed = turnSpeed; 
    }

    public override NodeStatus Execute()
    {
        if (enemyBT.target == null)
        {
            return NodeStatus.FAILURE;
        }
        
        TurnToward();
        return NodeStatus.SUCCESS;
    }
    public void TurnToward()
    {
        if (enemyBT.target != null)
        {
            Vector3 targetDirection = enemyBT.target.transform.position - enemyBT.transform.position;
            if (targetDirection.magnitude > 1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                enemyBT.transform.rotation = Quaternion.Slerp(enemyBT.transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            }
        }
    }
}
