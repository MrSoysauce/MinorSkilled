using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : AnimatorAIBase
{
    public Vector3 goal;

    public float speedModifier = 1;
    public float angularSpeedModifier = 1;
    public float finishRange = 1;

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

        NavMeshPath p = new NavMeshPath();
        if (!agent.CalculatePath(agent.desiredVelocity, p))
            anim.SetTrigger("ReachedGoal");

        if (agent.remainingDistance < finishRange || agent.isStopped)
            anim.SetTrigger("ReachedGoal");
    }

    protected override void OnStop(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetAgentSpeed();
    }
}
