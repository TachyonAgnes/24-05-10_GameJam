using UnityEngine;


public class EnemyBT : MonoBehaviour
{
    public bool debug = false;

    [HideInInspector] public Transform target;

    protected EnemyBT enemyBT;
    protected SelectorNode root;

    protected virtual void Awake()
    {
    }


    // Considering some nodes use rigidbody, FixedUpdate is used instead of Update
    protected virtual void FixedUpdate()
    {
    }
}
