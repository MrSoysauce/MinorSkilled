using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabable : RaycastInteractable
{
    private Rigidbody rb;
    private Collider col;

    private void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public void Grab()
    {
        rb.isKinematic = true;
        col.enabled = false;
    }

    public void UnGrab()
    {
        rb.isKinematic = false;
        col.enabled = true;
    }
}
