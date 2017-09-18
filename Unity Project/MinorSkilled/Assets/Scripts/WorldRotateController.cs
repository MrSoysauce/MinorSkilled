using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotateAxis
{
    X,
    Y,
    Z
}

public class WorldRotateController : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;

    [SerializeField] private List<Rigidbody> freezeBodies;
    [SerializeField] private PlayerController player;

    private PhysicsGroup[] groups;

    private Coroutine coroutine;

    private void Start()
    {
        groups = GetComponentsInChildren<PhysicsGroup>();
    }

    public void RotateWorld(RotateAxis rotateAxis, float rotateAmount)
    {
        if (coroutine == null)
            coroutine = StartCoroutine(rotateWorld(rotateAxis, rotateAmount));
    }

    private void FreezeObjects()
    {
        player.canMove = false;
        player.useGravity = false;
        player.rb.velocity = Vector3.zero;

        foreach (Rigidbody body in freezeBodies)
        {
            body.isKinematic = true;
        }
        foreach (PhysicsGroup group in groups)
        {
            group.Freeze();
        }
    }

    private void UnFreezeObjects()
    {
        player.canMove = true;
        player.useGravity = true;
        player.rb.velocity = Vector3.zero;

        foreach (Rigidbody body in freezeBodies)
        {
            body.isKinematic = false;
        }
        foreach (PhysicsGroup group in groups)
        {
            group.UpdateGravity();
            group.UnFreeze();
        }
    }

    private IEnumerator rotateWorld(RotateAxis rotateAxis, float rotateAmount)
    {
        FreezeObjects();

        Quaternion rot = transform.rotation;
        float counter = 0;
        int direction = (int)Mathf.Sign((int) rotateAmount);

        //Slowly rotate
        while (counter < Mathf.Abs((float)rotateAmount))
        {
            counter += Time.deltaTime*rotateSpeed;

            float rotation = Time.deltaTime*rotateSpeed*direction;
            Vector3 r = new Vector3();
            switch (rotateAxis)
            {
                case RotateAxis.X:
                    r.x = rotation;
                    break;
                case RotateAxis.Y:
                    r.y = rotation;
                    break;
                case RotateAxis.Z:
                    r.z = rotation;
                    break;
            }

            transform.Rotate(r);
            yield return null;
        }
        //Confirm rotation
        transform.rotation = rot;
        transform.Rotate((float)rotateAmount, 0, 0);

        UnFreezeObjects();

        coroutine = null;
    }
}
