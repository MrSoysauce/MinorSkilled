using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : AnimatorAIHelper
{
    [Header("Enemy General")]
    [SerializeField] protected PlayerInteractions player = null;
    [SerializeField] protected Transform visuals;
    [SerializeField] private bool onlyDrawGizmosOnSelected = false;

    [Header("Player interaction")]
    [SerializeField] protected float hearingRadius;
    [SerializeField] private bool drawHearingRadius;

    [SerializeField] protected float seeingRadius;
    [SerializeField] protected float seeingAngle;
    [SerializeField] private bool drawSeeingFrustrum;

    [SerializeField] protected float interactionRadius;
    [SerializeField] private bool drawInteractionRadius;

    protected bool allowedToMove = true;
    protected bool chasing = false;

    protected Vector3 heardPosition;

    protected override void Start()
    {
        base.Start();
        Debug.Assert(player != null,
            "PlayerController field in " + name + " is null! Please make sure to set it.");
    }

    public virtual bool CanSee(Collider obj)
    {
        Vector3 directionToTarget = obj.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        if (chasing || Mathf.Abs(angle) < seeingAngle/2.0f)
        {
            Transform[] raycastPoints = player.raycastPoints;
            foreach (Transform t in raycastPoints)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, (t.transform.position - transform.position).normalized, out hit, seeingRadius))
                    if (hit.collider == obj)
                        return true;
            }
        }

        return false;
    }

    protected void Update()
    {
        if (!allowedToMove)
            return;

        CheckSounds();
        CheckSight();
        UpdateEnemy();
        SetAnimatorValues();
    }

    protected void CheckSight()
    {
        bool canSee = CanSee(player.col);
        chasing = canSee;
        animator.SetBool("CanSeePlayer", canSee);
    }

    private void CheckSounds()
    {
        animator.SetBool("HeardSomething", false);

        if (!agent.isOnNavMesh || !agent.isActiveAndEnabled)
            return;
        
        ////Check if we are able to get to the position we heard
        //NavMeshPath p = new NavMeshPath();
        //if (!agent.CalculatePath(player.transform.position, p))
        //    return;

        //Check if we are in range of the player
        float playerSoundRange = player.GetSoundRange();
        if (Math.Abs(playerSoundRange) > float.Epsilon && Vector3.Distance(transform.position, player.transform.position) < hearingRadius + player.GetSoundRange())
        {
            animator.SetBool("HeardSomething", true);
            heardPosition = player.transform.position;
        }
    }

    protected virtual void UpdateEnemy()
    {

    }

    protected virtual void SetAnimatorValues()
    {
        if (chasing)
            heardPosition = player.transform.position;
        //It doesn't matter if we're actually chasing something or not, we can always try to set the state.
        //It is more safe to assume that our knowledge here can be different than mecanim
        Chase[] chase = animator.GetBehaviours<Chase>();
        foreach (Chase c in chase)
            c.goal = chasing ? player.transform.position : heardPosition;

        //TODO: Other animator values
    }

    private void OnDrawGizmosSelected()
    {
        if (onlyDrawGizmosOnSelected)
            DrawGiz();
    }

    private void OnDrawGizmos()
    {
        if (!onlyDrawGizmosOnSelected)
            DrawGiz();
    }

    private void DrawGiz()
    {
        //Hearing
        Gizmos.color = Color.blue;

        if (drawHearingRadius)
            Gizmos.DrawWireSphere(transform.position, hearingRadius);

        Gizmos.color = Color.yellow;

        //Seeing
        Matrix4x4 temp = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        if (drawSeeingFrustrum)
            Gizmos.DrawFrustum(Vector3.zero, seeingAngle, seeingRadius, 0, 1);
        Gizmos.matrix = temp;

        //Attacking
        Gizmos.color = Color.red;
        if (drawInteractionRadius)
            Gizmos.DrawWireSphere(transform.position, interactionRadius);

        Gizmos.color = Color.white;
    }
}
