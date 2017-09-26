using UnityEngine;

public class Enemy1 : AnimatorAIHelper
{
    [Space(10)]
    [Header("General")]
    [SerializeField] public GameObject visuals;
    [SerializeField] protected PlayerInteractions player;

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
        bool inrange = Vector3.Distance(transform.position, player.transform.position) < radius;

        bool allowedToWalk;
        if (origin == null)
            allowedToWalk = true;
        else
            allowedToWalk = Vector3.Distance(origin.position, player.transform.position) < originRange;

        bool canSee = false;
        if (inrange && allowedToWalk)
        {
            if (requiresVision)
            {
                RaycastHit hit;
                foreach (Transform p in player.raycastPoints)
                {
                    if (Physics.Raycast(visuals.transform.position, p.position - visuals.transform.position, out hit, radius))
                    {
                        if (hit.transform.CompareTag("Player"))
                        {
                            canSee = true;
                            break;
                        }
                    }
                }
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
        if (Vector3.Distance(transform.position, player.transform.position) < catchPlayerRadius)
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
