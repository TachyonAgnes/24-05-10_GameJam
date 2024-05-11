using UnityEngine;
using UnityEngine.AI;

public class ReturnToPatrolNode : ActionNode
{
    private Transform target;
    private NavMeshAgent agent;

    public ReturnToPatrolNode(Transform target, NavMeshAgent agent)
    {
        this.target = target;
        this.agent = agent;
    }

    public override NodeStatus Execute()
    {
        if (target == null)
        {
            return NodeStatus.FAILURE;
        }

        // if the agent is close enough to the target, return success
        if (Vector3.Distance(agent.transform.position, target.position) < 3f)
        {
            return NodeStatus.SUCCESS;
        }

        agent.destination = target.position;
        return NodeStatus.RUNNING;
    }
}
