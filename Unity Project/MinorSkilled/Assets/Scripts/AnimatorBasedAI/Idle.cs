using UnityEngine;

public class Idle : AnimatorAIBase
{
    protected override void OnStart(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetAgentSpeed(0, 0);
    }

    protected override void OnUpdate(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    protected override void OnStop(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
