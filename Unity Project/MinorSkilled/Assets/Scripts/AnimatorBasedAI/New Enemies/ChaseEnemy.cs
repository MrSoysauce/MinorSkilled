using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEnemy : EnemyBase
{
    protected override void UpdateEnemy()
    {
        base.UpdateEnemy();

        if (Vector3.Distance(transform.position, player.transform.position) < interactionRadius)
        {
            player.RespawnPlayer();
        }
    }
}
