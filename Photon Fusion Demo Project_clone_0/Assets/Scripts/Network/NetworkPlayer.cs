using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    [SerializeField] private GameObject objectTest;
    
    public static NetworkPlayer Local { get; set; }

    private void Awake()
    {
        Local = this;
    }

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
            objectTest.SetActive(false);
    }

    public void PlayerLeft(PlayerRef player) { }
}
