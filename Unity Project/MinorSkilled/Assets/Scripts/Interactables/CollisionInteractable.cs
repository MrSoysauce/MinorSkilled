using UnityEngine;

public abstract class CollisionInteractable : InteractableBase
{
    private void OnCollisionEnter(Collision col)
    {
        OnStartInteract(col.gameObject);
    }

    private void OnTriggerEnter(Collider col)
    {
        OnStartInteract(col.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        OnInteract(other.gameObject);
    }

    private void OnCollisionStay(Collision other)
    {
        OnInteract(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        OnEndInteract(other.gameObject);
    }

    private void OnCollisionExit(Collision other)
    {
        OnEndInteract(other.gameObject);
    }
}
