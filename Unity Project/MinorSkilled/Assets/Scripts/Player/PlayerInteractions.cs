using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    private NPCInteractable npc;
    private PlayerController player;

    [SerializeField] private GameObject interactMessage;

    private void Start()
    {
        if (interactMessage != null)
            interactMessage.SetActive(false);

        player = GetComponent<PlayerController>();
    }

    /// <summary>
    /// Set to null to disable
    /// </summary>
    /// <param name="interactable"></param>
    public void SetNPCInRange(NPCInteractable interactable)
    {
        this.npc = interactable;

        if (interactMessage != null)
        {
            if (npc == null)
                interactMessage.SetActive(false);
            else if (npc != null)
                interactMessage.SetActive(true);
        }
    }

    private void Update()
    {
        bool interactInput = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button0);
        if (interactInput)
            Interact();
    }

    private void Interact()
    {
        //Yo we have an npc
        if (npc != null)
        {
            //Disable on start
            player.canMove = false;
            player.allowJump = false;

            //Do our cool thing
            //wot

            //enable after
        }
    }
}
