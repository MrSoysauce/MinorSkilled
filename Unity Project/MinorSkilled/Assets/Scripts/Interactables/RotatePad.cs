using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePad : CollisionInteractable
{
    [SerializeField] private RotateAmount amount;

    protected override void OnInteract(GameObject interacting)
    {
        if (interacting.CompareTag("Player"))
        {
            //VERY MUCH TEMPORARY
            interacting.transform.Translate(0, 2, 0);
            WorldRotateController.instance.RotateWorld(amount);
        }
    }
}
