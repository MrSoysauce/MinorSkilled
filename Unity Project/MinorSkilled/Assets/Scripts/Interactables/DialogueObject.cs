﻿using UnityEngine;

public class DialogueObject : CollisionInteractable
{
    private GameObject dialogueUI;

    private bool playerCanInteract = false;

    private PlayerController talking = null;

    private void Start()
    {
        Debug.Assert(dialogueUI != null, "DialogueUI is null!");
        dialogueUI.SetActive(false);
    }

    protected override void OnEndInteract(GameObject interacting)
    {
        if (interacting.CompareTag("Player"))
        {
            playerCanInteract = true;
            PlayerInteractions p = GetPlayerInteractionsFailSafe(interacting);
            p.SetActiveDialogue(this);
        }
    }

    protected override void OnStartInteract(GameObject interacting)
    {
        if (interacting.CompareTag("Player"))
        {
            playerCanInteract = false;
            PlayerInteractions p = GetPlayerInteractionsFailSafe(interacting);
            p.SetActiveDialogue(null);
        }
    }

    private void Update()
    {
        if (!playerCanInteract)
            return;
    }

    private PlayerInteractions GetPlayerInteractionsFailSafe(GameObject p)
    {
        PlayerInteractions player;
        player = p.GetComponent<PlayerInteractions>();
        if (player != null)
            return player;

        player = p.GetComponentInParent<PlayerInteractions>();
        if (player != null)
            return player;

        player = p.GetComponentInChildren<PlayerInteractions>();
        if (player != null)
            return player;

        return player;
    }

    public void StartDialogue(PlayerController talking)
    {
        this.talking = talking;
        talking.canMove = false;
        talking.allowJump = false;

        //Enable dialogue
        dialogueUI.SetActive(true);
    }

    /// <summary>
    /// Supposed to be called by the unity event system
    /// </summary>
    public void EndDialogue()
    {
        talking.allowJump = true;
        talking.canMove = true;

        //Disable dialogue
        dialogueUI.SetActive(false);
    }
}