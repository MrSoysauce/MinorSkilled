using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class AnimatorAIHelper : MonoBehaviour
{
    [SerializeField] public List<Waypoints> waypoints;
    [SerializeField] public int editID;
    [SerializeField] public Transform wayPointTransform;

    [SerializeField] private NavMeshAgent agent;
    private Animator animator;

    private void Start()
    {
        //Get animator
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
