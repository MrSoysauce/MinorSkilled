using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionInteractable : InteractableBase
{
    [SerializeField] private bool isTrigger;

    private void OnCollisionEnter(Collision col)
    {
        if (isTrigger)
            return;

        OnInteract(col.gameObject);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!isTrigger)
            return;

        OnInteract(col.gameObject);
    }
}
