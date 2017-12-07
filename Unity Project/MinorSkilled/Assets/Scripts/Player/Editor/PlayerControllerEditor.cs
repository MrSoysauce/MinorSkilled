using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    private SerializedObject obj;
    private bool drawMovement = false;

    public override void OnInspectorGUI()
    {
        if (obj == null)
            obj = new SerializedObject(target);

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.PropertyField(obj.FindProperty("drag"));
        EditorGUILayout.PropertyField(obj.FindProperty("gravity"));
        EditorGUILayout.PropertyField(obj.FindProperty("useGravity"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.PropertyField(obj.FindProperty("walkSpeed"));
        EditorGUILayout.PropertyField(obj.FindProperty("stairsGravityModifier"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.PropertyField(obj.FindProperty("runModifier"));
        EditorGUILayout.PropertyField(obj.FindProperty("sprintDrainSpeed"));
        EditorGUILayout.PropertyField(obj.FindProperty("sprintRegainSpeed"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.PropertyField(obj.FindProperty("sneakModifier"));
        EditorGUILayout.PropertyField(obj.FindProperty("crouchDetectionDistance"));
        EditorGUILayout.PropertyField(obj.FindProperty("crouchDetectLayer"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.PropertyField(obj.FindProperty("groundedLayerMask"));
        EditorGUILayout.PropertyField(obj.FindProperty("groundedDetectRange"));

        EditorGUILayout.PropertyField(obj.FindProperty("jumpMode"));

        obj.ApplyModifiedProperties();
        switch ((target as PlayerController).jumpMode)
        {
            case PlayerController.JumpMode.Controlled:
                EditorGUILayout.PropertyField(obj.FindProperty("jumpStrength"));
                EditorGUILayout.PropertyField(obj.FindProperty("controlledJumpTime"));
                break;
            case PlayerController.JumpMode.MultiJump:
                EditorGUILayout.PropertyField(obj.FindProperty("multiJumps"));
                EditorGUILayout.PropertyField(obj.FindProperty("groundedJumpStrength"));
                EditorGUILayout.PropertyField(obj.FindProperty("midairJumpStrength"));
                EditorGUILayout.PropertyField(obj.FindProperty("multiJumpDelay"));
                break;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.PropertyField(obj.FindProperty("midairModifier"));
        EditorGUILayout.PropertyField(obj.FindProperty("midairDrag"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.PropertyField(obj.FindProperty("grabModifier"));
        EditorGUILayout.PropertyField(obj.FindProperty("grabDistance"));
        EditorGUILayout.PropertyField(obj.FindProperty("throwForce"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.PropertyField(obj.FindProperty("climbDistance"));
        EditorGUILayout.PropertyField(obj.FindProperty("climbSpeed"));
        EditorGUILayout.PropertyField(obj.FindProperty("climbLayer"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.PropertyField(obj.FindProperty("slideTime"));
        EditorGUILayout.EndHorizontal();

        GUILayout.BeginVertical("Box");
        EditorGUILayout.PropertyField(obj.FindProperty("visuals"));

        GUI.color = drawMovement ? Color.green : Color.red;
        if (GUILayout.Button("Draw debug movement info"))
            drawMovement = !drawMovement;
        GUI.color = Color.white;

        GUILayout.EndHorizontal();

        if (drawMovement)
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(obj.FindProperty("verticalInput"));
            EditorGUILayout.PropertyField(obj.FindProperty("horizontalInput"));
            EditorGUILayout.PropertyField(obj.FindProperty("canMove"));

            EditorGUILayout.PropertyField(obj.FindProperty("canJump"));

            EditorGUILayout.PropertyField(obj.FindProperty("jumpInput"));
            EditorGUILayout.PropertyField(obj.FindProperty("jumpInputPressed"));
            EditorGUILayout.PropertyField(obj.FindProperty("sprintInput"));
            EditorGUILayout.PropertyField(obj.FindProperty("crouchInput"));
            EditorGUILayout.PropertyField(obj.FindProperty("climbInput"));
            EditorGUILayout.PropertyField(obj.FindProperty("isMoving"));
            EditorGUILayout.PropertyField(obj.FindProperty("sprintCharge"));

            GUILayout.EndHorizontal();
        }

        obj.ApplyModifiedProperties();
    }
}
