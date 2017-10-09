using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppingGrabPlayerEnemy : GrabPlayerEnemy
{
    [Header("Dropping enemy specific")]
    [SerializeField] private float upsideDownVisualsOffset;
    [SerializeField] private float descendSpeed;
    [SerializeField] private Transform descendTransform;

    private bool dropped;

    protected override void UpdateEnemy()
    {
        float visOffset = dropped ? 0 : upsideDownVisualsOffset;
        Vector3 offset = new Vector3(0, visOffset, 0);
        visuals.localPosition = offset;

        if (!dropped)
            return;

        base.UpdateEnemy();
    }

    public void Drop() //Used in event manager
    {
        if (dropped)
            return;

        StartCoroutine(DropSelf());
    }

    private IEnumerator DropSelf()
    {
        Vector3 startDescend = transform.position;
        Vector3 descendEnd = new Vector3(transform.position.x, descendTransform.position.y, transform.position.z);

        float t = 0;
        while (t <= 1)
        {
            agent.enabled = false;

            t += Time.deltaTime * descendSpeed;
            transform.position = Vector3.Lerp(startDescend, descendEnd, t);

            CheckForPlayer();
            if (attached)
            {
                dropped = true;
                visuals.transform.localRotation = Quaternion.identity;
                yield break;
            }

            yield return null;
        }

        visuals.transform.localRotation = Quaternion.identity;

        dropped = true;

        agent.enabled = true;
        animator.SetBool("Dropped", true);
    }

    public override bool CanSee(Collider obj)
    {
        return dropped
            ? base.CanSee(obj)
            : Vector3.Distance(transform.position, player.transform.position) < hearingRadius;
    }
}
