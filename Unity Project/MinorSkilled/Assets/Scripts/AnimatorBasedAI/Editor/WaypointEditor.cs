using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Waypoints))]
public class WaypointEditor : Editor
{
    private static void CreateAsset<T>(string path) where T : ScriptableObject
    {
        //Create
        T asset = CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);

        //Save asset
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("Assets/Create/Waypoints")]
    public static void CreateWaypoint()
    {
        //Get path and save it there
        string path = EditorUtility.SaveFilePanelInProject("Save Waypoints instance", "new waypoint", "asset", String.Empty);
        CreateAsset<Waypoints>(path);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SceneView.onSceneGUIDelegate = OnScene;
    }

    private static void OnScene(SceneView sceneview)
    {
        Waypoints o = Selection.activeObject as Waypoints;
        if (o == null)
            return;

        if (o.waypoints == null)
            o.waypoints = new List<Vector3>();

        for (int i = 0; i < o.waypoints.Count; i++)
        {
            Vector3 wp = o.waypoints[i];

            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.PositionHandle(wp, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(o, "Change Waypoint pos");
                o.waypoints[i] = newTargetPosition;
                EditorUtility.SetDirty(o);
            }

            Color c = Handles.color;
            if (o.ordered)
            {
                if (i == 0)
                    Handles.color = Color.green;
                if (i == o.waypoints.Count - 1)
                    Handles.color = Color.red;

                if (i != 0)
                    Debug.DrawLine(o.waypoints[i - 1], o.waypoints[i]);

                if (i == 0 || i == o.waypoints.Count-1)
                {   
                    Handles.FreeMoveHandle(o.waypoints[i], Quaternion.identity,
                        HandleUtility.GetHandleSize(o.waypoints[i])/10, Vector3.zero, Handles.SphereHandleCap);
                }

                if (o.loop)
                {
                    if (i == o.waypoints.Count - 1)
                        Debug.DrawLine(o.waypoints[i], o.waypoints[0], Color.red);
                }
            }
        }
    }
}