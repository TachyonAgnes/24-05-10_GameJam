using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// this node is used to check if there is an enemy in the area
// first check if there is an enemy in the sphere area
// then check if there is an obstacle between the agent and the target

public class CheckTargetInAreaNode : ConditionNode
{
    private EnemyBT enemyBT;
    private float detectRadius;
    private float detectAngle;
    private NavMeshAgent agent;
    private float moveSpeed_fast;
    private float moveSpeed_normal;
    private LayerMask obstacleLayerMask;

    // initialize
    public CheckTargetInAreaNode(EnemyBT enemyBT, float detectRadius, float detectAngle,
        NavMeshAgent agent, float moveSpeed_fast, float moveSpeed_normal, LayerMask obstacleLayerMask)
    {
        this.enemyBT = enemyBT;
        this.detectRadius = detectRadius;
        this.detectAngle = detectAngle;
        this.agent = agent;
        this.moveSpeed_fast = moveSpeed_fast;
        this.moveSpeed_normal = moveSpeed_normal;
        this.obstacleLayerMask = obstacleLayerMask;
    }
    public override NodeStatus Execute()
    {
        if (CheckEnemiesInArea())
        {
            return NodeStatus.SUCCESS;
        }
        else
        {
            return NodeStatus.FAILURE;
        }
    }

    // check if there is an enemy in the area
    private bool CheckEnemiesInArea()
    {
        LayerMask mask = LayerMask.GetMask("Player");
        Collider[] colliders = Physics.OverlapSphere(enemyBT.transform.position, detectRadius, mask);

        Collider closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            if(col.transform.name == "PlayerArmature")
            {
                continue;
            }

            // if there is an obstacle between the agent and the target, skip
            if (!CheckObstacleBetween(enemyBT.transform, col.transform) && CheckTargetInDetectAngle(enemyBT.transform, col.transform))
            {
                float distance = Vector3.Distance(enemyBT.transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestEnemy = col;
                    closestDistance = distance;
                }
            }
        }

        // if there is an enemy in the area, set the target to the closest enemy
        if (closestEnemy != null)
        {
            enemyBT.target = closestEnemy.transform;
            if (enemyBT.debug)
            {
                Debug.DrawRay(closestEnemy.transform.position, Vector3.up, Color.red, 5f);
            }

            // increase movement speed
            agent.speed = moveSpeed_fast;
            return true;
        }

        // no enemy detected, drop target.
        enemyBT.target = null;
        agent.speed = moveSpeed_normal;
        return false;
    }

    // Check if there is an obstacle between the agent and the target
    private bool CheckObstacleBetween(Transform self, Transform target)
    {
        Vector3 start = self.position;
        Vector3 end = target.position - start;
        float maxDistance = end.magnitude + 2f;
        bool result = Physics.Raycast(start, end, out RaycastHit hit, maxDistance, obstacleLayerMask) && hit.collider.gameObject != target.gameObject;

        return result;
    }

    // Check if target is in the attacking angle
    private bool CheckTargetInDetectAngle(Transform self, Transform target)
    {
        Vector3 directionToTarget = target.position - self.position;
        float angle = Vector3.Angle(self.forward, directionToTarget);
        if(enemyBT.debug){
            Debug.DrawLine(self.position, self.position + self.forward * 10, Color.blue, 0.5f);
            Debug.DrawLine(self.position, self.position + directionToTarget.normalized * 10, Color.green, 0.5f);
        }
        return angle < detectAngle / 2;
    }
}
