using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private NetworkCharacterControllerPrototype networkCharacterController;
    [SerializeField] private float speed = 15f;
    
    [Networked] public NetworkButtons ButtonsPrevious { get; set; }
    
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // move
            Vector3 moveVector = data.movementInput.normalized;
            networkCharacterController.Move(moveVector * speed * Runner.DeltaTime);
            
            // jump
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);
            ButtonsPrevious = buttons;
            
            if (pressed.IsSet(InputButtons.JUMP))
                networkCharacterController.Jump();
        }
    }
}
