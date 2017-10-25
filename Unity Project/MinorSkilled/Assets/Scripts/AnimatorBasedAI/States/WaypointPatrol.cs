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
        AnimatorAIHelper helper = GetHelper(anim);
        wp = helper.waypoints[waypointID];
        waypointsToGo = new List<Vector3>(wp.waypoints);
        finishedPatrolling = false;

        Patrol(false);
    }

    protected override void OnUpdate(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Patrol();
    }

    protected override void OnStop(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        finishedPatrolling = false;
    }

    private void Patrol(bool checkForDistance = true)
    {
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;

        //Did we get to the waypoint yet?
        if (checkForDistance)
        {
            bool finished = agent.remainingDistance < finishRange;
            if (!finished)
                return;
        }

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

        Transform wpt = agent.GetComponent<AnimatorAIHelper>().wayPointTransform;

        //Get new waypoint
        int i;
        Vector3 newDestination;
        if (wp.ordered)
        {
            newDestination = wpt.TransformPoint(waypointsToGo[0]);
            i = 0;
        }
        else
        {
            i = Random.Range(0, waypointsToGo.Count);
            newDestination = wpt.TransformPoint(waypointsToGo[i]);
        }
        waypointsToGo.RemoveAt(i);

        //Set new destination
        agent.destination = newDestination;
    }
}