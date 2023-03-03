using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    [SerializeField] private Transform playerCamera;
    
    public static NetworkPlayer Local { get; set; }

    private void Awake()
    {
        Local = this;
    }

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
        {
            playerCamera.gameObject.SetActive(false);
        }
        else
        {
            playerCamera.parent = null;
            
            Camera mainCamera = Camera.main;
            mainCamera.enabled = false;

            AudioListener audioListener = FindObjectOfType<AudioListener>();
            audioListener.enabled = false;
        }
    }

    public void PlayerLeft(PlayerRef player) { }
}
