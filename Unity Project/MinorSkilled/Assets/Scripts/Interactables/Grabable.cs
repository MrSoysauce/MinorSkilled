using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabable : RaycastInteractable
{
    private Rigidbody rb;
    private Collider col;
    private bool grabbed;

    public bool liftable = true;
    public Vector3 liftingOffset = new Vector3(0, 1, 0);

    private void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }
    
    public void Grab()
    {
        grabbed = true;
        rb.isKinematic = true;
        col.enabled = false;
    }

    private void Update()
    {
        if (grabbed && transform.parent.CompareTag("Player"))
        {
            if (liftable)
                transform.localPosition = liftingOffset;
        }
    }

    public void UnGrab()
    {
        grabbed = false;
        rb.isKinematic = false;
        col.enabled = true;
    }
}
