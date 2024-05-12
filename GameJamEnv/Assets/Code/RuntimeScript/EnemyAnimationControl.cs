using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationControl : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyBT_0 enemyBT;

    private void Start()
    {
        enemyBT.attackTargetNode.OnEnemyAttack += AttackTargetNode_OnEnemyAttack;
    }

    private void AttackTargetNode_OnEnemyAttack()
    {
        Debug.Log("Attack event triggered");
        animator.SetTrigger("Attack");
    }

    private void LateUpdate()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

}
