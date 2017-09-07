using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsGroupObject : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 grav;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetGravity(Vector3 grav)
    {
        this.grav = grav;
    }

    private void FixedUpdate()
    {
        rb.AddForce(grav);
    }
}
