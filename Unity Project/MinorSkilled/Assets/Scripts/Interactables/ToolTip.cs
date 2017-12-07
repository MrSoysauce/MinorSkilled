using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class ToolTip : CollisionInteractable
{
    [SerializeField] private GameObject toolTip;

    private void Start()
    {
        if (toolTip == null)
            Debug.LogError("Tooltip object is null!");
        else
            toolTip.SetActive(false);
    }

    protected override void OnStartInteract(GameObject interacting)
    {
        if (!interacting.CompareTag("Player"))
            return;

        toolTip.SetActive(true);
        base.OnInteract(interacting);
    }

    protected override void OnEndInteract(GameObject interacting)
    {
        if (!interacting.CompareTag("Player"))
            return;
        toolTip.SetActive(false);
        base.OnEndInteract(interacting);
    }
}
*/