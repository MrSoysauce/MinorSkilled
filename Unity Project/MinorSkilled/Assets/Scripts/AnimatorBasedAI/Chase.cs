using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : AnimatorAIBase
{

    private Transform player;
    private Dummy dummy;

    protected override void OnStart(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameManager.Instance.player.transform;
        dummy = anim.gameObject.GetComponent<Dummy>();
        SetAgentSpeed(dummy.chaseSpeed,agent.angularSpeed);
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
        agent.destination = player.position;
    }
}
