using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    public enum MoveTypes { waypoints, manual }

    [SerializeField] public MoveTypes type;
    [SerializeField] private float speed = 1;
    //Manual
    [HideInInspector] public float distance;
    [HideInInspector] public Vector3 direction;
    //Waypoint values
    [HideInInspector] public List<Vector3> waypoints = new List<Vector3>();
    [HideInInspector] public bool loop;
    private Vector3 startingPos;
    private int nextWaypoint;
    private Vector3 destination;

    private float checkDistance = 0.5f;
    private float counter = 0;

    void Start()
    {
        if (type == MoveTypes.waypoints)
        {
            if (waypoints != null && waypoints.Count > 0)
            {
                transform.position = waypoints[0];
                SetDestination(waypoints[0]);
            }
        }
        startingPos = transform.position;
    }

	void Update ()
	{
	    if (type == MoveTypes.manual)
	    {
	        counter+=Time.deltaTime;
	        transform.position = startingPos + direction.normalized * Mathf.PingPong(counter*speed, distance);
	    }
	    else
	    {
            //Cache transform pos
	        Vector3 oldPos = transform.position;
            //Move platform
	        transform.position += direction * speed * Time.deltaTime;
            //Check if we moved past the destination or we reached the destination
	        if (ReachedDestination() ||
                Vector3.Distance(oldPos, destination) < Vector3.Distance(transform.position, destination))
	        {
	            nextWaypoint++;
                //Check if waypoints ran out
	            if (nextWaypoint == waypoints.Count)
	            {
                    nextWaypoint = 0;
                    enabled = true;
	            }
	             SetDestination(waypoints[nextWaypoint]);
	        }

	    }
    }

    private bool ReachedDestination()
    {
        return Vector3.Distance(transform.position, destination) <= checkDistance;
    }

    private void SetDestination(Vector3 targetDestination)
    {
        destination = targetDestination;
        direction = destination - transform.position;
        direction.Normalize();
    }
}

[CustomEditor(typeof(MovingPlatform))]
[CanEditMultipleObjects]
public class MovingPlatformEditor : Editor
{
    private MovingPlatform platform;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        platform = (MovingPlatform) target;
        switch (platform.type)
        {
            case MovingPlatform.MoveTypes.manual:
                platform.distance = EditorGUILayout.FloatField("Distance",platform.distance);
                platform.direction = EditorGUILayout.Vector3Field("Direction",platform.direction);
                break;
            case MovingPlatform.MoveTypes.waypoints:
                GUILayout.BeginHorizontal();
                GUI.color = Color.green;
                if (GUILayout.Button("Add waypoint"))
                {
                    platform.waypoints.Add(new Vector3(platform.transform.position.x,platform.transform.position.y + 1,platform.transform.position.z));
                }
                GUI.color = Color.red;
                if (GUILayout.Button("Remove waypoint") && platform.waypoints.Count > 0) platform.waypoints.RemoveAt(platform.waypoints.Count-1);
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
                for (int i = 0; i < platform.waypoints.Count; i++)
                {
                    platform.waypoints[i] = EditorGUILayout.Vector3Field("Waypoint "+(i+1), platform.waypoints[i]);
                }
                break;
        }
    }

    void OnSceneGUI()
    {
        platform = (MovingPlatform)target;
        switch (platform.type)
        {
            case MovingPlatform.MoveTypes.waypoints:
                DrawWaypointsOnScene();
                break;
            case MovingPlatform.MoveTypes.manual:
                Debug.DrawLine(platform.transform.position,platform.transform.position + platform.distance * platform.direction.normalized,Color.red);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DrawWaypointsOnScene()
    {
        for (int i = 0; i < platform.waypoints.Count; i++)
        {
            Vector3 wp = platform.waypoints[i];

            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.PositionHandle(wp, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed waypoint position");
                platform.waypoints[i] = newTargetPosition;
            }

            Color c = Handles.color;
            if (i == 0)
                Handles.color = Color.green;
            if (i == platform.waypoints.Count - 1)
                Handles.color = Color.red;

            if (i != 0)
                Debug.DrawLine(platform.waypoints[i - 1], platform.waypoints[i]);

            if (i == 0 || i == platform.waypoints.Count - 1)
            {
                Handles.FreeMoveHandle(platform.waypoints[i], Quaternion.identity,
                HandleUtility.GetHandleSize(platform.waypoints[i]) / 10, Vector3.zero, Handles.SphereHandleCap);
            }

        }
    }
}
