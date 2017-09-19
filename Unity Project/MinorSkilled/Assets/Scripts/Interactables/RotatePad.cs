using UnityEngine;

public class RotatePad : CollisionInteractable
{
    [SerializeField] private RotateAxis rotateAxis;
    [SerializeField] private float rotateAmount;
    [SerializeField] private bool activateWithButton = true;

    [SerializeField] private WorldRotateController rotateController;
    protected override void OnInteract(GameObject interacting)
    {
        if (interacting.CompareTag("Player"))
        {
            if (activateWithButton)
            {
                PlayerInteractions p = interacting.GetComponent<PlayerInteractions>();
                if (p == null)
                    p = interacting.GetComponentInParent<PlayerInteractions>();

                p.SetRotatePad(this);
            }
            else 
                Apply();
        }
    }

    protected override void OnEndInteract(GameObject interacting)
    {
        if (activateWithButton)
        {
            if (interacting.CompareTag("Player"))
            {
                PlayerInteractions p = interacting.GetComponent<PlayerInteractions>();
                if (p == null)
                    p = interacting.GetComponentInParent<PlayerInteractions>();

                p.SetRotatePad(null);
            }
        }
    }

    public void Apply()
    {
        rotateController.RotateWorld(rotateAxis, rotateAmount);
    }
}
