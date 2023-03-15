using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Fusion;
using UnityEngine;

public class PlayerCameraHandler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Update()
    {
        NetworkPlayer[] networkPlayer = FindObjectsOfType<NetworkPlayer>();

        foreach (var player in networkPlayer)
        {
            if (player.Object.HasInputAuthority)
                cinemachineVirtualCamera.Follow = player.transform;
        }
    }
}
