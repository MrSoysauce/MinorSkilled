using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : AnimatorAIBase
{
    public Vector3 goal;

    public float speedModifier = 1;
    public float angularSpeedModifier = 1;
    protected override void OnStart(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetAgentSpeed(agentDefaultSpeed * speedModifier, agentDefaultAngularSpeed * angularSpeedModifier);
    }

    protected override void OnUpdate(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ChaseTarget(anim);
    }

    private void ChaseTarget(Animator anim)
    {
        SetAgentSpeed(agentDefaultSpeed * speedModifier, agentDefaultAngularSpeed * angularSpeedModifier);

        //TODO: why is agent not enabled sometimes when it's chasing?
        //Quick fix
        if (!agent.enabled) agent.enabled = true;

        agent.destination = goal;
    }

    protected override void OnStop(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetAgentSpeed();
    }
}
