using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePad : CollisionInteractable
{
    [SerializeField] private RotateAxis rotateAxis;
    [SerializeField] private float rotateAmount;

    [SerializeField] private WorldRotateController rotateController;
    protected override void OnInteract(GameObject interacting)
    {
        if (interacting.CompareTag("Player"))
        {
            //VERY MUCH TEMPORARY
            rotateController.RotateWorld(rotateAxis, rotateAmount);
        }
    }
}
