using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    protected virtual void OnStartInteract(GameObject interacting) { }
    protected virtual void OnInteract(GameObject interacting) { }
    protected virtual void OnEndInteract(GameObject interacting) { }
}
