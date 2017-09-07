using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : AnimatorAIBase
{
    private List<Vector3> waypointsToGo;

    private Animator animator;
    private Transform player;
    private Dummy dummy;

    protected override void OnStart(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator = anim;
        player = GameManager.Instance.player.transform;
        dummy = anim.gameObject.GetComponent<Dummy>();
        waypointsToGo = new List<Vector3>(dummy.waypoints.waypoints);
        if (player == null) Debug.LogError("The player property of Patrol has not been set",anim.gameObject);
    }

    protected override void OnUpdate(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Walk();
        CheckVision();
    }

    private void Walk()
    {
        //Can we get to the waypoint?
        if (agent.isStopped)
        {
            //Skip that destination
            Vector3 nextDestination = GetNextWaypoint();
            SetDestination(nextDestination);
        }

        //Did we get to the waypoint yet?
        bool finished = agent.remainingDistance < dummy.waypointCheckDistance + agent.stoppingDistance;
        if (!finished)
            return;

        //Out of waypoints?
        if (waypointsToGo.Count == 0)
        {
            //Restart
            if (dummy.waypoints.loop)
                waypointsToGo = new List<Vector3>(dummy.waypoints.waypoints);
            //Stop moving
            else
            {
                SetAgentSpeed(0, 0);
                return;
            }
        }

        //Get new waypoint
        Vector3 newDestination = GetNextWaypoint();
        //Set new destination
        SetDestination(newDestination);
    }

    private void SetDestination(Vector3 newDestination)
    {
        //Find point on NavMesh which is the closest to our random point
        NavMeshHit hit;
        NavMesh.SamplePosition(newDestination, out hit, float.PositiveInfinity, 1);
        Vector3 finalPosition = hit.position;

        //Set new destination
        agent.destination = finalPosition;
    }

    private Vector3 GetNextWaypoint()
    {
        Vector3 newDestination;
        if (dummy.waypoints.ordered)
            newDestination = waypointsToGo[0];
        else
            newDestination = waypointsToGo[Random.Range(0, waypointsToGo.Count)];
        waypointsToGo.Remove(newDestination);
        return newDestination;
    }

    private void CheckVision()
    {
        Vector3 dummyPosition = dummy.transform.position + dummy.visionOffset;
        RaycastHit hit;
        //Is target within detection range?
        if (Vector3.Distance(player.position, dummyPosition) < dummy.detectionRange)
        {
            Vector3 rayDirection = player.position - dummyPosition;
            Vector3 dummyForward = dummy.transform.forward;
            Vector3 rayForward = new Vector3(rayDirection.x,0,rayDirection.z);
            dummyForward.y = 0;
            Debug.Log("Target within range");
            //Is target within field of vision?
            if (Vector3.Angle(rayForward, dummyForward) < dummy.fov/2)
            {
                Debug.Log("Target within field of vision");
                //Are any objects obscuring our vision
                if (Physics.Raycast(dummyPosition, rayDirection, out hit, 100))
                {
                    Debug.Log("Looking at target");
                    if (hit.transform == player) animator.SetBool("Chasing",true);
                }
            }
        }
    }
}