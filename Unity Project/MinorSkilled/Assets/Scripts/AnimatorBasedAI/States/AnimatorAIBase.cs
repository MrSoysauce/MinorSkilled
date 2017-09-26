using UnityEngine;

public abstract class AnimatorAIBase : StateMachineBehaviour
{
    public Color debugColor;

    [HideInInspector] public UnityEngine.AI.NavMeshAgent agent;

    [HideInInspector] public float agentDefaultSpeed;
    [HideInInspector] public float agentDefaultAngularSpeed;

    protected void ResetAgentSpeed()
    {
        agent.speed = agentDefaultSpeed;
        agent.angularSpeed = agentDefaultAngularSpeed;
    }

    protected void SetAgentSpeed(float speed, float angularSpeed)
    {
        agent.speed = speed;
        agent.angularSpeed = angularSpeed;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!IsValid(animator))
            return;

        foreach(MeshRenderer m in animator.GetComponent<AnimatorAIHelper>().changeColorRenderers)
        {
            m.material.color = debugColor;
            m.material.SetColor("_EmissionColor", debugColor);
        }

        ResetAgentSpeed();
        OnStart(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!IsValid(animator))
            return;

        OnUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!IsValid(animator))
            return;

        ResetAgentSpeed();
        OnStop(animator, stateInfo, layerIndex);
    }

    protected virtual void OnStart(Animator anim, AnimatorStateInfo stateInfo, int layerIndex) {}
    protected virtual void OnUpdate(Animator anim, AnimatorStateInfo stateInfo, int layerIndex) {}
    protected virtual void OnStop(Animator anim, AnimatorStateInfo stateInfo, int layerIndex) {}

    private bool IsValid(Animator animator)
    {
        if (agent == null)
        {
            AnimatorAIHelper helper = animator.GetComponent<AnimatorAIHelper>();
            if (helper != null) helper.AssignAgent();
            else Debug.LogError("Please assign an AnimatorAIHelper to " + animator.gameObject.name,animator.gameObject);

            if (agent == null)
            {
                Debug.LogError("Please assign an NavMeshAgent to " + animator.gameObject.name, animator.gameObject);
                return false;
            }
        }

        return true;
    }
}
