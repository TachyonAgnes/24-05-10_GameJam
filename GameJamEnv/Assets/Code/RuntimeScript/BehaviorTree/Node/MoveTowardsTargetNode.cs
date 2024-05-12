using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MoveTowardsTargetNode : ActionNode
{
    private EnemyBT enemyBT;
    private NavMeshAgent agent;

    public MoveTowardsTargetNode(EnemyBT enemyBT, NavMeshAgent agent)
    {
        this.enemyBT = enemyBT;
        this.agent = agent;
    }

    public override NodeStatus Execute() 
    {
        if (enemyBT.target != null)
        {
            Vector3 targetPosition = enemyBT.target.position;
            agent.SetDestination(targetPosition);
            return NodeStatus.RUNNING; 
        }
        else
        {
            return NodeStatus.FAILURE;
        }
    }

}
