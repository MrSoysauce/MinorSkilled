using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotateAmount
{
    Deg90Plus = 90,
    Deg90Min = -90,
    Deg180Plus = 180,
    Deg180Min = -180
}

public class WorldRotateController : MonoBehaviour
{
    public static WorldRotateController instance;

    [SerializeField] private float rotateSpeed;

    [SerializeField] private List<Rigidbody> freezeBodies;
    [SerializeField] private PlayerController player;

    private PhysicsGroup[] groups;

    private Coroutine coroutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        groups = GetComponentsInChildren<PhysicsGroup>();
    }

    public void RotateWorld(RotateAmount amount)
    {
        if (coroutine == null)
            coroutine = StartCoroutine(rotateWorld(amount));
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

    private IEnumerator rotateWorld(RotateAmount amount)
    {
        FreezeObjects();

        Quaternion rot = transform.rotation;
        float counter = 0;
        int direction = (int)Mathf.Sign((int) amount);

        //Slowly rotate
        while (counter < Mathf.Abs((float)amount))
        {
            counter += Time.deltaTime*rotateSpeed;
            transform.Rotate(Time.deltaTime*rotateSpeed*direction, 0, 0);
            yield return null;
        }
        //Confirm rotation
        transform.rotation = rot;
        transform.Rotate(90, 0, 0);

        UnFreezeObjects();

        coroutine = null;
    }
}
