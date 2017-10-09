using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerEffectType
{
    Confuse,
    Drain
}

public class GrabPlayerEnemy : EnemyBase
{
    [SerializeField] private PlayerEffectType effectType;
    [SerializeField] private float attachedHeight = 1.1f;

    [Header("Confuse eff")]
    [SerializeField] private bool invertControls;
    [SerializeField] private bool forceMovement;
    [SerializeField] private float confuseSpeedMod;

    [Header("Drain eff")]
    [SerializeField] private float drainSpeed;

    protected bool attached = false;
    private Coroutine drain;

    protected override void UpdateEnemy()
    {
        CheckForPlayer();

        if (attached)
            UpdateAttached();
    }

    private void UpdateAttached()
    {
        //Make sure we stay in the correct position
        if (attached)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            visuals.transform.localPosition = new Vector3(0, attachedHeight, 0);
            agent.enabled = false;
        }
    }

    private void Attach()
    {
        if (player.CanAttachEnemy() == false)
            return;

        attached = true;
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero;
        visuals.transform.localPosition = new Vector3(0, attachedHeight, 0);
        agent.enabled = false; //important

        player.AttachEnemy(this);

        animator.SetBool("Attached", true);
    }

    public void Detach()
    {
        if (drain != null)
        {
            StopCoroutine(drain);
            drain = null;
        }

        animator.SetBool("Attached", false);

        DestroyObject(gameObject);
    }

    public void ApplyEffects(PlayerController p)
    {
        switch (effectType)
        {
            case PlayerEffectType.Confuse:
                float modSpeed = 1;
                if (invertControls)
                    modSpeed *= -1;
                modSpeed *= confuseSpeedMod;
                p.scriptableSpeedModifier = modSpeed;
                p.forceMovement = forceMovement;
                break;
            case PlayerEffectType.Drain:
                if (drain == null)
                    drain = StartCoroutine(DrainSpeed(p));
                break;
        }
    }

    private IEnumerator DrainSpeed(PlayerController p)
    {
        float mod = 1;
        while (mod > 0)
        {
            mod -= Time.deltaTime * drainSpeed;
            p.scriptableSpeedModifier = mod;
            player.SetFadeAlpha(mod); //Temporary
            yield return null;
        }

        //Player ded
        //TODO: Animations and stuff
        drain = null;
        player.RespawnPlayer(); //Reloads scene so we don't really *have* to detach
    }

    protected void CheckForPlayer()
    {
        if (!attached && Vector3.Distance(transform.position, player.transform.position) < interactionRadius)
            Attach();
    }
}