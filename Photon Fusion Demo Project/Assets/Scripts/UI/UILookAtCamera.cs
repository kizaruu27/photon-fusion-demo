using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;

    private void Update()
    {
        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
        transform.LookAt(playerCamera.transform);
    }
}
