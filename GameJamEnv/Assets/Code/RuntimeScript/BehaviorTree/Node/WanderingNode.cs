using UnityEngine;
using UnityEngine.AI;

public class WanderingNode : ActionNode
{
private float wanderRadius;
    private float wanderTimer;
    private float wanderTimerMax;
    private Vector3 wanderPoint;
    private EnemyBT enemyBT;
    private NavMeshAgent agent;
    public WanderingNode(EnemyBT enemyBT, float wanderRadius, float wanderTimer, NavMeshAgent agent)
    {
        this.enemyBT = enemyBT;
        this.wanderRadius = wanderRadius;
        this.wanderTimer = wanderTimer;
        this.wanderTimerMax = wanderTimer;
        this.agent = agent;
    }
    public override NodeStatus Execute()
    {
        // if target is found, skip wandering
        if (enemyBT.target != null) {
            return NodeStatus.FAILURE;
        }

        if (wanderTimer >= wanderTimerMax)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += enemyBT.transform.position;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, -1);

            wanderPoint = navHit.position;
            agent.SetDestination(wanderPoint);
            wanderTimer = 0;
        }
        else
        {
            wanderTimer += Time.deltaTime;
        }


        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                return NodeStatus.SUCCESS;
            }
        }

        return NodeStatus.RUNNING;
    }
   
}
