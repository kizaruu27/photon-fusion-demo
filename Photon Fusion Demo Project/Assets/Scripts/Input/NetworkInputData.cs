using UnityEngine;
using Fusion;

public enum InputButtons
{
    JUMP
}

public struct NetworkInputData : INetworkInput
{
    public Vector3 movementInput;
    public NetworkButtons buttons;
}
