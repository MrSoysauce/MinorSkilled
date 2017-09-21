using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : AnimatorAIBase
{
    public Vector3 goal;

    protected override void OnStart(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    protected override void OnUpdate(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ChaseTarget(anim);
    }

    private void ChaseTarget(Animator anim)
    {
        //TODO: why is agent not enabled sometimes when it's chasing?
        //Quick fix
        if (!agent.enabled) agent.enabled = true;

        agent.destination = goal;
    }
}
