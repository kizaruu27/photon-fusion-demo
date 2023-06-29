using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Fusion;
using TMPro;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] public NetworkCharacterControllerPrototype networkCharacterController;
    [SerializeField] private float speed = 15f;
    [SerializeField] public bool isChatting = false;

    [SerializeField] private MeshRenderer meshRenderer;
    
    [Networked(OnChanged = nameof(OnHpChanged))]
    public int Hp { get; set; }

    [SerializeField] private int maxHP = 100;

    [SerializeField] private Image hpBar;

    [Networked] public NetworkButtons ButtonsPrevious { get; set; }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isChatting)
        {
            ChangeColor_RPC(Color.red);
        }
        if (Input.GetKeyDown(KeyCode.X) && !isChatting)
        {
            ChangeColor_RPC(Color.blue);
        }
        if (Input.GetKeyDown(KeyCode.C) && !isChatting)
        {
            ChangeColor_RPC(Color.yellow);
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data) && !isChatting)
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
            
            // fire
            if (pressed.IsSet(InputButtons.FIRE))
                Runner.Spawn(bulletPrefab, transform.position + transform.TransformDirection(Vector3.forward),
                    Quaternion.LookRotation(transform.TransformDirection(Vector3.forward)), Object.InputAuthority);
            
            // check if hp is below 0
            if (Hp <= 0 || networkCharacterController.transform.position.y <= -5)
                Respawn();
        }
    }

    static void OnHpChanged(Changed<PlayerController> changed)
    {
        changed.Behaviour.hpBar.fillAmount = (float)changed.Behaviour.Hp / changed.Behaviour.maxHP;
    }

    private void Respawn()
    {
        networkCharacterController.transform.position = Vector3.up * 2;
        Hp = maxHP;
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
            Hp = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)
            Hp -= damage;
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void ChangeColor_RPC(Color newColor)
    {
        meshRenderer.material.color = newColor;
    }
}
