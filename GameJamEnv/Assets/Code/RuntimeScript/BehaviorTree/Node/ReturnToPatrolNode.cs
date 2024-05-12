using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ReturnToPatrolNode : ActionNode
{
    private Transform target;
    private NavMeshAgent agent;
    private EnemyBT enemyBT;
    private float thresholdDistance = 8.0f;

    public ReturnToPatrolNode(EnemyBT enemyBT, Transform target, NavMeshAgent agent, float thresholdDistance)
    {
        this.target = target;
        this.agent = agent;
        this.enemyBT = enemyBT;
        this.thresholdDistance = thresholdDistance;
    }

    public override NodeStatus Execute()
    {
        // Check the current distance to the target
        float currentDistance = Vector3.Distance(agent.transform.position, target.position);

        // If the agent is within 8 meters of the target, consider this node complete
        if (currentDistance < thresholdDistance)
        {
            return NodeStatus.SUCCESS; // Close enough, move to the next node in the sequence
        }

        // If the agent is farther than 8 meters, set the destination and keep running
        agent.SetDestination(target.position);
        return NodeStatus.RUNNING; // Continue running this node
    }
}
