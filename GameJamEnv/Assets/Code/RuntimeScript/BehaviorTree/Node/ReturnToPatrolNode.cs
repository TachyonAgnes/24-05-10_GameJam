using UnityEngine;
using UnityEngine.AI;

public class ReturnToPatrolNode : ActionNode
{
    private Transform target;
    private NavMeshAgent agent;
    private EnemyBT enemyBT;

    public ReturnToPatrolNode(EnemyBT enemyBT, Transform target, NavMeshAgent agent)
    {
        this.target = target;
        this.agent = agent;
        this.enemyBT = enemyBT;
    }

    public override NodeStatus Execute()
    {
        // if the agent is close enough to the target, return success
        if (Vector3.Distance(agent.transform.position, target.position) < 8f)
        {
            agent.ResetPath();
            return NodeStatus.SUCCESS;
        }
        agent.destination = target.position;
        return NodeStatus.SUCCESS;
    }
}
