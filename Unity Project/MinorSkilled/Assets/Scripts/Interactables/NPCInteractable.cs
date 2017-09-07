using UnityEngine;

public class NPCInteractable : CollisionInteractable
{
    protected override void OnStartInteract(GameObject interacting)
    {
        if (interacting != null && interacting.CompareTag("Player"))
        {
            PlayerController pc = interacting.GetComponent<PlayerController>();
            if (pc != null) pc.allowJump = false;

            PlayerInteractions pi = interacting.GetComponent<PlayerInteractions>();
            if (pi != null) pi.SetNPCInRange(this);
        }
    }

    protected override void OnEndInteract(GameObject interacting)
    {
        if (interacting.CompareTag("Player"))
        {
            PlayerController pc = interacting.GetComponent<PlayerController>();
            if (pc != null) pc.allowJump = true;

            PlayerInteractions pi = interacting.GetComponent<PlayerInteractions>();
            if (pi != null) pi.SetNPCInRange(null);
        }
    }
}
