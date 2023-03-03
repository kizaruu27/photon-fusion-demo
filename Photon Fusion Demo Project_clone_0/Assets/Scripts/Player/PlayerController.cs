using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private NetworkCharacterControllerPrototype networkCharacterController;

    [SerializeField] private float speed = 15f;
    
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            Vector3 moveVector = data.movementInput.normalized;
            networkCharacterController.Move(moveVector * speed * Runner.DeltaTime);
        }
    }
}
