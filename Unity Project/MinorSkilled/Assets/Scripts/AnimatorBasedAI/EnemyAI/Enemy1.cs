using UnityEngine;

public class Enemy1 : AnimatorAIHelper
{
    [Space(10)]
    [SerializeField] public GameObject visuals;
    [SerializeField] protected Transform player;

    [Header("Player detection")]
    [SerializeField] protected bool drawRadius;
    [SerializeField] protected float radius;
    [SerializeField] protected bool requiresVision;

    [Header("Player catching")]
    [SerializeField] protected bool drawCatchPlayerRange;
    [SerializeField] protected float catchPlayerRadius = 0.5f;

    [Header("Origin")]
    [Tooltip("If origin is set, the enemy will try to stay in the range of origin when trying to chase the player")]
    [SerializeField] protected Transform origin;
    [SerializeField] protected float originRange;
    [SerializeField] protected bool drawOriginRange;

    protected override void Start()
    {
        base.Start();

        if (player == null)
            Debug.LogError("Player is null in " + name);
    }

    protected virtual void Update()
    {
        bool inrange = Vector3.Distance(transform.position, player.position) < radius;

        bool allowedToWalk;
        if (origin == null)
            allowedToWalk = true;
        else
            allowedToWalk = Vector3.Distance(origin.position, player.position) < originRange;

        bool canSee = false;
        if (inrange && allowedToWalk)
        {
            if (requiresVision)
            {
                RaycastHit hit;
                canSee = Physics.Raycast(transform.position, player.position - transform.position, out hit, radius);
            }
            else
            {
                canSee = true;
            }
        }

        animator.SetBool("CanSeePlayer", canSee);
        foreach (Chase c in animator.GetBehaviours<Chase>())
            c.goal = player.transform.position;

        inRangeCheck();
    }

    protected virtual void inRangeCheck()
    {
        if (Vector3.Distance(transform.position, player.position) < catchPlayerRadius)
            player.GetComponent<PlayerInteractions>().RespawnPlayer();
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (drawRadius)
            Gizmos.DrawSphere(transform.position, radius);

        Gizmos.color = Color.red;
        if (drawCatchPlayerRange)
            Gizmos.DrawSphere(transform.position, catchPlayerRadius);

        Gizmos.color = Color.green;
        if (origin && drawOriginRange)
            Gizmos.DrawSphere(origin.position, originRange);
    }
}
