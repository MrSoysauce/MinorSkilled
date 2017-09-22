using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimatorAIHelper))]
public class AIHelperEditor : Editor
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

    protected virtual void OnSceneGUI()
    {
        Waypoints o = null;

        AnimatorAIHelper helper = (AnimatorAIHelper)target;
        if (helper.waypoints != null)
        {
            for (int i = 0; i < helper.waypoints.Count; i++)
            {
                Waypoints wp = helper.waypoints[i];
                if (wp != null && i == helper.editID)
                    o = wp;
            }
        }

        if (o == null)
            return;

        for (int i = 0; i < o.waypoints.Count; i++)
        {
            Vector3 wp = o.waypoints[i];

            EditorGUI.BeginChangeCheck();
            Matrix4x4 mat = helper.wayPointTransform.localToWorldMatrix;

            Vector3 newTargetPosition = Handles.PositionHandle(mat.MultiplyVector(wp), Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(o, "Change Waypoint pos");
                o.waypoints[i] = (mat.inverse.MultiplyVector(newTargetPosition));
                EditorUtility.SetDirty(o);
            }

            if (o.ordered)
            {
                if (i == 0)
                    Handles.color = Color.green;
                if (i == o.waypoints.Count - 1)
                    Handles.color = Color.red;

                if (i != 0)
                    Debug.DrawLine((mat.MultiplyVector(o.waypoints[i-1])), mat.MultiplyVector(o.waypoints[i]));

                if (i == 0 || i == o.waypoints.Count - 1)
                {
                    Handles.FreeMoveHandle(mat.MultiplyVector(o.waypoints[i]), Quaternion.identity,
                        HandleUtility.GetHandleSize(mat.MultiplyVector(o.waypoints[i])) / 10, Vector3.zero, Handles.SphereHandleCap);
                }

                if (o.loop)
                {
                    if (i == o.waypoints.Count - 1)
                        Debug.DrawLine(mat.MultiplyVector(o.waypoints[i]), mat.MultiplyVector(o.waypoints[0]), Color.red);
                }
            }
        }
    }
}

[CustomEditor(typeof(Enemy1))]
public class Enemy1Editor : AIHelperEditor
{
    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();
    }
}

[CustomEditor(typeof(Enemy2))]
public class Enemy2Editor : Enemy1Editor
{
	protected override void OnSceneGUI()
	{
		base.OnSceneGUI();
	}
}