using UnityEngine;
using UnityEngine.AI;

public class EnemyBT_0 : EnemyBT
{
    [SerializeField]
    private float moveSpeed_normal = 3f;
    [SerializeField]
    private float moveSpeed_fast = 6.5f;
    [SerializeField]
    private float detectRadius = 10f;
    [SerializeField]
    private float detectAngle = 120f;
    [SerializeField]
    private float attackingAngle = 60f;
    [SerializeField]
    private float attackingRange = 2f;
    [SerializeField]
    private float executionInterval = 0.3f;
    [SerializeField]
    private float turnSpeed = 1f;
    [SerializeField]
    private Transform patrolPosition;

    private NavMeshAgent navMeshAgent;
    
    public AttackTargetNode attackTargetNode { get; private set; }

    [ContextMenu("Drop Target")]
    public void DropTarget()
    {
        target = null;
    }

    protected override void Start()
    {
        enemyBT = this;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed_normal;

        root = new SelectorNode();
        root.debug = debug;

        ParallelNode continuousPatrolAndAttack = new ParallelNode();
        continuousPatrolAndAttack.AddChild(new CheckTargetInAreaNode(enemyBT, detectRadius, detectAngle, navMeshAgent, moveSpeed_fast, moveSpeed_normal));
        SequenceNode offensiveModuleSeq = new SequenceNode();
        offensiveModuleSeq.AddChild(new MoveTowardsTargetNode(enemyBT, navMeshAgent));

        attackTargetNode = new AttackTargetNode(enemyBT, attackingAngle, executionInterval, attackingRange);
        offensiveModuleSeq.AddChild(attackTargetNode);

        continuousPatrolAndAttack.AddChild(offensiveModuleSeq);

        // if target not found, navmesh needed to reset position
        SequenceNode targetLostSeq = new SequenceNode();
        targetLostSeq.AddChild(new ReturnToPatrolNode(enemyBT, patrolPosition, navMeshAgent, 8f));
        targetLostSeq.AddChild(new WanderingNode(enemyBT, 15f, 5f, navMeshAgent));



        // add the offensive module to the root
        root.AddChild(continuousPatrolAndAttack);
        // add the target lost sequence to the root
        root.AddChild(targetLostSeq);

    }

    // Considering some nodes use rigidbody, FixedUpdate is used instead of Update
    protected override void FixedUpdate()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 10, Color.blue, 0.5f);
        root.Execute();
    }
}
