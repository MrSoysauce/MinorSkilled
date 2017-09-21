using UnityEngine;

public class Enemy2 : Enemy1
{
    [HideInInspector] public bool attached = false;

    [Header("Player effects")]
    [SerializeField] public bool invertControls;
    [SerializeField] public float slow;
    [SerializeField] public bool forceMovement;

    protected override void Update()
    {
        animator.SetBool("Attached", attached);

        if (attached)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            visuals.transform.localPosition = new Vector3(0, 1.5f, 0);
            return;
        }

        base.Update();
    }

    protected override void inRangeCheck()
    {
        if (attached)
            return;

        if (Vector3.Distance(transform.position, player.position) < catchPlayerRadius)
        {
            if (!player.GetComponent<PlayerInteractions>().AttachEnemy(this))
                return;

            attached = true;
            animator.SetBool("Attached", attached);
        }
    }
}
