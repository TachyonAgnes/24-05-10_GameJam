using UnityEngine;

public class WanderingNode : ActionNode
{
private float wanderRadius;
    private float wanderTimer;
    private float wanderTimerMax;
    private Vector3 wanderPoint;
    private EnemyBT enemyBT;
    private float moveSpeed;
    public WanderingNode(EnemyBT enemyBT, float wanderRadius, float wanderTimer, float moveSpeed)
    {
        this.enemyBT = enemyBT;
        this.wanderRadius = wanderRadius;
        this.wanderTimer = wanderTimer;
        this.wanderTimerMax = wanderTimer;
        this.moveSpeed = moveSpeed;
    }
    public override NodeStatus Execute()
    {
        // if target is found, skip wandering
        if (enemyBT.target != null) {
            return NodeStatus.FAILURE;
        }

        if (wanderTimer >= wanderTimerMax)
        {
            Vector3 randomDirection = new Vector3(Random.Range(-wanderRadius, wanderRadius), 0, Random.Range(-wanderRadius, wanderRadius));
            wanderPoint = enemyBT.transform.position + randomDirection; 
            wanderTimer = 0;
        }
        else
        {
            wanderTimer += Time.deltaTime;
        }


        if (Vector3.Distance(enemyBT.transform.position, wanderPoint) < 3f)
        {
            return NodeStatus.SUCCESS;
        }
        else
        {
            MoveTo(wanderPoint);
            return NodeStatus.RUNNING;
        }
    }
   
    private void MoveTo(Vector3 position)
    {
        Vector3 direction = (position - enemyBT.transform.position).normalized;
        enemyBT.transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
