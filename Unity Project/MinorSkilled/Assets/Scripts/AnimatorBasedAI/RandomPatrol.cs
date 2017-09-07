using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomPatrol : AnimatorAIBase
{
    [SerializeField] private float finishRange = 1;

    [SerializeField] private float maxRangeToNewWayPoint = 40;

    [SerializeField] private float minDistanceFromCurrentPos = 5;
    [SerializeField] private float minDistanceFromOldPos = 5;

    protected override void OnStart(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    protected override void OnUpdate(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Wander();
    }

    protected override void OnStop(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    private void Wander()
    {
        //Did we get to our goalyet?
        bool finished = agent.remainingDistance < finishRange;

        if (finished)
        {
            bool done = false;

            Vector3 oldGoal = agent.destination;

            //Try to find a new waypoint, we are looping in case we accidentally find a waypoint without a path to it
            while (!done)
            {
                //Get our radius
                float radius = maxRangeToNewWayPoint;

                //Find a new point within our radius
                Vector3 newDestination = new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius)) + agent.transform.position;

                //Too close to current pos
                if (Vector3.Distance(agent.transform.position, newDestination) < minDistanceFromCurrentPos)
                    continue;
                //Too close to old pos
                if (Vector3.Distance(newDestination, oldGoal) < minDistanceFromOldPos)
                    continue;

                //Find point on NavMesh which is the closest to our random point
                NavMeshHit hit;
                NavMesh.SamplePosition(newDestination, out hit, float.PositiveInfinity, 1);
                Vector3 finalPosition = hit.position;

                //Set new destination
                agent.destination = finalPosition;

                //Calculate path and check if it is valid
                NavMeshPath path = new NavMeshPath();
                done = agent.CalculatePath(finalPosition, path);
            }
        }
    }
}
