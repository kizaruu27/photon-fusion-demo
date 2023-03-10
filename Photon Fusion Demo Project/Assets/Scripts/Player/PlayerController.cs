using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private NetworkCharacterControllerPrototype networkCharacterController;
    [SerializeField] private float speed = 15f;
    [SerializeField] private Camera playerCamera;

    [Networked] public NetworkButtons ButtonsPrevious { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // move
            Vector3 moveWorld = data.movementInput.normalized;
            //Vector3 moveSelf = playerCamera.transform.TransformDirection(moveWorld);
            networkCharacterController.Move(Runner.DeltaTime * speed * moveWorld);
            
            // jump
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);
            ButtonsPrevious = buttons;
            
            if (pressed.IsSet(InputButtons.JUMP))
                networkCharacterController.Jump();
            
            // fire
            if (pressed.IsSet(InputButtons.FIRE))
                Runner.Spawn(bulletPrefab, transform.localPosition + transform.TransformDirection(Vector3.forward),
                    Quaternion.LookRotation(transform.TransformDirection(Vector3.forward)), Object.InputAuthority);
        }
    }
}
