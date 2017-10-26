using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundModifyingMaterial : CollisionInteractable
{
    [SerializeField] private float modifier = 1;
    public float Modifier { get { return modifier; } }

    protected override void OnStartInteract(GameObject interacting)
    {
        PlayerInteractions p = interacting.GetComponent<PlayerInteractions>();
        if (p)
            p.sndmod = this;
    }

    protected override void OnEndInteract(GameObject interacting)
    {
        PlayerInteractions p = interacting.GetComponent<PlayerInteractions>();
        if (p &&  p.sndmod == this)
            p.sndmod = null;
    }
}
