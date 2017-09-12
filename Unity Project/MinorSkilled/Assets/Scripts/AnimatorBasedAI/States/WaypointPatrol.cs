using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : AnimatorAIBase
{
    [SerializeField] private int waypointID;
    private List<Vector3> waypointsToGo;
    [SerializeField] private float finishRange = 1;
    [HideInInspector] public bool finishedPatrolling = false;

    private Waypoints wp;

    protected override void OnStart(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorAIHelper helper = anim.GetComponent<AnimatorAIHelper>();
        wp = helper.waypoints[waypointID];
        waypointsToGo = new List<Vector3>(wp.waypoints);
        finishedPatrolling = false;
    }

    protected override void OnUpdate(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Patrol();
    }

    protected override void OnStop(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        finishedPatrolling = false;
    }

    private void Patrol()
    {
        //Did we get to the waypoint yet?
        bool finished = agent.remainingDistance < finishRange;
        if (!finished)
            return;

        //Out of waypoints?
        if (waypointsToGo.Count == 0)
        {
            //Restart
            if (wp.loop)
                waypointsToGo = new List<Vector3>(wp.waypoints);
            //Stop moving
            else
            {
                finishedPatrolling = true;
                SetAgentSpeed(0, 0);
                return;
            }
        }

        //Get new waypoint
        Vector3 newDestination;
        if (wp.ordered)
            newDestination = waypointsToGo[0];
        else
            newDestination = waypointsToGo[Random.Range(0, waypointsToGo.Count)];
        waypointsToGo.Remove(newDestination);

        //Find point on NavMesh which is the closest to our random point
        NavMeshHit hit;
        NavMesh.SamplePosition(newDestination, out hit, float.PositiveInfinity, 1);
        Vector3 finalPosition = hit.position;

        //Set new destination
        agent.destination = finalPosition;
    }
}