using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabable : RaycastInteractable
{
    private Rigidbody rb;
    private Collider col;
    private bool grabbed;

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
            transform.localPosition = new Vector3(0, 2.5f, 0);
    }

    public void UnGrab()
    {
        grabbed = false;
        rb.isKinematic = false;
        col.enabled = true;
    }
}
