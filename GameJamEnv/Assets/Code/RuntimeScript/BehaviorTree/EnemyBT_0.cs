using UnityEngine;

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

    protected override void Start()
    {
        enemyBT = this;

        root = new SelectorNode();
        root.debug = debug;

        // create a sequence node to check if there is an enemy in the area
        SequenceNode patrolSeq = new SequenceNode();
        patrolSeq.AddChild(new CheckTargetInAreaNode(enemyBT, detectRadius, detectAngle));

        // if success
        // create a paralleNode do move and attack
        ParallelNode offensiveModuleParalle = new ParallelNode();
        offensiveModuleParalle.AddChild(new MoveTowardsTargetNode(enemyBT, moveSpeed));
        offensiveModuleParalle.AddChild(new AttackTargetNode(enemyBT, attackingAngle, executionInterval));
        offensiveModuleParalle.AddChild(new LookAtTargetNode(enemyBT, detectRadius, detectAngle, turnSpeed));

        // if target not found, A* needed to reset position

        // add the patrol sequence to the root
        root.AddChild(patrolSeq);
        // add the offensive module to the root
        root.AddChild(offensiveModuleParalle);

    }

    // Considering some nodes use rigidbody, FixedUpdate is used instead of Update
    protected override void FixedUpdate()
    {
        root.Execute();
    }
}
