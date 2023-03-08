using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;
using Behaviour = Fusion.Behaviour;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private TextMeshProUGUI playerNicknameText;

    [Networked(OnChanged = nameof(OnNicknameChanged))]
    public NetworkString<_16> nickname { get; set; }

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
            RPC_SetNickname(PlayerPrefs.GetString("PlayerNickname"));
            
            playerCamera.parent = null;

            Camera mainCamera = Camera.main;
            mainCamera.enabled = false;

            AudioListener audioListener = FindObjectOfType<AudioListener>();
            audioListener.enabled = false;
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
    }

    void OnNicknameChanged()
    {
        Debug.Log($"Nickname changed for player to {nickname} for player {gameObject.name}");
        playerNicknameText.text = nickname.ToString();
    }

    static void OnNicknameChanged(Changed<NetworkPlayer> changed)
    {
        Debug.Log($"{Time.time} On nickname changed value {changed.Behaviour.nickname}");
        changed.Behaviour.OnNicknameChanged();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickname(string nickname, RpcInfo info = default)
    {
        Debug.Log($"[RPC] set nickname {nickname}");
        this.nickname = nickname;
    }

}
