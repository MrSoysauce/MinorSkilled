using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [Header("Patrol")]
    public Waypoints waypoints;
    public float waypointCheckDistance = 1;
    public float chaseSpeed = 1;

    [Header("Vision")]
    public Vector3 visionOffset;
    public float detectionRange = 1;
    public float fov = 90;

    [HideInInspector] public bool inCombat;
    private Transform player;

    void Awake()
    {
        player = GameManager.Instance.player.transform;
    }

    void Update()
    {
        if (!inCombat) return;

        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
        targetRotation = Quaternion.Euler(new Vector3(0, targetRotation.eulerAngles.y, 0));
        transform.rotation = targetRotation;
    }

    void OnDrawGizmosSelected()
    {
        Matrix4x4 temp = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawFrustum(visionOffset, fov, detectionRange, 0.01f, 1);
        Gizmos.matrix = temp;
    }
}