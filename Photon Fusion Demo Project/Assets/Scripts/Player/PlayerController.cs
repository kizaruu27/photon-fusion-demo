using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private NetworkCharacterControllerPrototype networkCharacterController;
    [SerializeField] private float speed = 15f;
    [SerializeField] private Animator anim;

    [SerializeField] private MeshRenderer meshRenderer;
    
    [Networked(OnChanged = nameof(OnHpChanged))]
    public int Hp { get; set; }

    [SerializeField] private int maxHP = 100;

    [SerializeField] private Image hpBar;


    [Networked] public NetworkButtons ButtonsPrevious { get; set; }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ChangeColor_RPC(Color.red);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeColor_RPC(Color.blue);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeColor_RPC(Color.yellow);
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // move
            Vector3 moveVector = data.movementInput.normalized;
            networkCharacterController.Move(moveVector * speed * Runner.DeltaTime);

            // animation
            bool isWalking = moveVector.magnitude != 0;
            bool isGround = networkCharacterController.IsGrounded;
            anim.SetBool("isWalking", isWalking && isGround);

            if (isGround)
            {
                // anim.SetBool("isWalking", false);
            }

            // if (moveVector.magnitude != 0)
            //     anim.SetBool("isWalking", true);
            // else
            //     anim.SetBool("isWalking", false);

            // jump
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);
            ButtonsPrevious = buttons;

            if (pressed.IsSet(InputButtons.JUMP))
            {
                networkCharacterController.Jump();
                anim.Play("Jumping Up");
            }
            
            // fire
            if (pressed.IsSet(InputButtons.FIRE))
                Runner.Spawn(bulletPrefab, transform.position + transform.TransformDirection(Vector3.forward),
                    Quaternion.LookRotation(transform.TransformDirection(Vector3.forward)), Object.InputAuthority);
            
            // check if hp is below 0
            if (Hp <= 0 || networkCharacterController.transform.position.y <= -5)
                Respawn();
        }
        else
        {
            // anim.Play("Idle");

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
