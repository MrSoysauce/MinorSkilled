using System.Collections.Generic;
using UnityEngine;

public class PhysicsGroup : MonoBehaviour
{
    private List<PhysicsGroupObject> objects = new List<PhysicsGroupObject>();
    private List<Rigidbody> rbs = new List<Rigidbody>();

    [SerializeField] private Vector3 customGravity;
    [SerializeField] private bool affectedByRotation = true;

    void Start()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].useGravity = false;
            PhysicsGroupObject obj = rigidbodies[i].GetComponent<PhysicsGroupObject>();
            if (obj == null)
                obj = rigidbodies[i].gameObject.AddComponent<PhysicsGroupObject>();

            objects.Add(obj);
            obj.SetGravity(customGravity);

            rbs.Add(rigidbodies[i]);
        }

    }

    public void SetGravity(Vector3 gravity)
    {
        Vector3 grav = gravity;
        if (affectedByRotation)
            grav = transform.TransformDirection(gravity);

        customGravity = gravity;
        foreach (PhysicsGroupObject o in objects)
        {
            o.SetGravity(grav);
        }
    }

    public void UpdateGravity()
    {
        SetGravity(customGravity);
    }

    public void Freeze()
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
        }
    }

    public void UnFreeze()
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
        }
    }
}
