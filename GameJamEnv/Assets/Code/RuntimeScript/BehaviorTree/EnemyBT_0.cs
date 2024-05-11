using UnityEngine;
using UnityEngine.AI;

public class EnemyBT_0 : EnemyBT
{
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private float detectRadius = 10f;
    [SerializeField]
    private float detectAngle = 120f;
    [SerializeField]
    private float attackingAngle = 60f;
    [SerializeField]
    private float executionInterval = 0.1f;
    [SerializeField]
    private float turnSpeed = 1f;
    [SerializeField]
    private Transform patrolPosition;

    private NavMeshAgent navMeshAgent;
    

    [ContextMenu("Drop Target")]
    public void DropTarget()
    {
        target = null;
    }

    protected override void Start()
    {
        enemyBT = this;
        navMeshAgent = GetComponent<NavMeshAgent>();

        root = new SelectorNode();
        root.debug = debug;

        ParallelNode continuousPatrolAndAttack = new ParallelNode();
        continuousPatrolAndAttack.AddChild(new CheckTargetInAreaNode(enemyBT, detectRadius, detectAngle));
        SequenceNode offensiveModuleSeq = new SequenceNode();
        offensiveModuleSeq.AddChild(new MoveTowardsTargetNode(enemyBT, moveSpeed));
        offensiveModuleSeq.AddChild(new LookAtTargetNode(enemyBT, detectRadius, detectAngle, turnSpeed));
        offensiveModuleSeq.AddChild(new AttackTargetNode(enemyBT, attackingAngle, executionInterval));
        continuousPatrolAndAttack.AddChild(offensiveModuleSeq);

        // if target not found, navmesh needed to reset position
        SequenceNode targetLostSeq = new SequenceNode();
        targetLostSeq.AddChild(new WanderingNode(enemyBT, 5f, 5f, moveSpeed));
        targetLostSeq.AddChild(new ReturnToPatrolNode(enemyBT, patrolPosition, navMeshAgent));

        //targetLostSeq.AddChild(new LookAtTargetNode(enemyBT, detectRadius, detectAngle, turnSpeed));



        // add the offensive module to the root
        root.AddChild(continuousPatrolAndAttack);
        // add the target lost sequence to the root
        root.AddChild(targetLostSeq);

    }

    // Considering some nodes use rigidbody, FixedUpdate is used instead of Update
    protected override void FixedUpdate()
    {
        root.Execute();
    }
}
