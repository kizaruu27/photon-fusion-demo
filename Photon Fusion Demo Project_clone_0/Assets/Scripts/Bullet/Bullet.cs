using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float bulletSpeed = 5f;
    [Networked] private TickTimer life { get; set; }

    public override void Spawned()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5f);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired((Runner)))
        {
            Runner.Despawn(Object);
        }
        else
        {
            // move the bullet
            transform.position += bulletSpeed * transform.forward * Runner.DeltaTime;
        }
    }
}
