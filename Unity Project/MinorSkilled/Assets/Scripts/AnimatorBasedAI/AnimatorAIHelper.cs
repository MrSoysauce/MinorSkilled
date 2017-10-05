using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimatorAIHelper : MonoBehaviour
{
    [Header("AI General")]
    [SerializeField] public List<Waypoints> waypoints;
    [SerializeField] public int editID;
    [SerializeField] public Transform wayPointTransform;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;

    [Header("Debug")]
    [SerializeField] public MeshRenderer[] changeColorRenderers;

    protected virtual void Start()
    {
        //Get animator
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("AnimatorAIHelper (" + name + ").animator is null! Please make sure that there's an animator component attached to this object!",this);
            return;
        }

        //Get agent
        if (agent == null)
        {
            Debug.Log("AnimatorAIHelper (" + name + ").navMeshAgent is null! Using GetComponent to get it.");
            agent = GetComponent<NavMeshAgent>();
        }

        AssignAgent();
    }

    public void AssignAgent()
    {
        //Give all our scripts a reference to agent
        AnimatorAIBase[] scripts = animator.GetBehaviours<AnimatorAIBase>();
        foreach (AnimatorAIBase script in scripts)
        {
            script.agent = agent;
            script.agentDefaultSpeed = agent.speed;
            script.agentDefaultAngularSpeed = agent.angularSpeed;
        }
    }
}
