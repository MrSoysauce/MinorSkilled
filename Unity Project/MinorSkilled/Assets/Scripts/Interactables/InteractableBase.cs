using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    protected abstract void OnInteract(GameObject interacting);
}
