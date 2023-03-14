using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using TMPro;
using Fusion;

[System.Serializable]
public static class PlayerDataContainer
{
    public static string playerName;
}

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    [SerializeField] private Transform cameraGroup;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CinemachineVirtualCamera cinemachineCam;
    [SerializeField] private TextMeshProUGUI playerNicknameText;
    [SerializeField] private MessageUI localUI;

    public LocalCamera localCamera;

    [Networked(OnChanged = nameof(OnNicknameChanged))]
    public NetworkString<_16> nickname { get; set; }

    public static NetworkPlayer Local { get; set; }

    private bool isPublicJoinMessegeSent = false;

    private void Awake()
    {
        Local = this;
        localUI = GameObject.Find("UI Canvas").GetComponent<MessageUI>();
    }

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
        {
            playerCamera.enabled = false;
            cinemachineCam.enabled = false;

            AudioListener audioListener = FindObjectOfType<AudioListener>();
            audioListener.enabled = false;

            // disable UI for remote player
            //localUI.SetActive(false);
        }
        else
        {
            RPC_SetNickname(PlayerDataContainer.playerName);

            cameraGroup.parent = null;

            Camera mainCamera = Camera.main;
            mainCamera.enabled = false;
        }
        
        Runner.SetPlayerObject(Object.InputAuthority, Object);
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Object.HasStateAuthority)
        {
            if (Object.HasStateAuthority)
            {
                if (Runner.TryGetPlayerObject(player, out NetworkObject playerLeftNetworkObject))
                {
                    if (playerLeftNetworkObject == Object)
                        Local.localUI.SendMessage(playerLeftNetworkObject.GetComponent<NetworkPlayer>().nickname.ToString(), " left the session");
                }
            }
        }
        
        // if (Object.InputAuthority)
        //     Runner.Despawn(Object);
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

        if (!isPublicJoinMessegeSent)
        {
            Local.localUI.SendMessage(nickname.ToString(), " joined the session");

            isPublicJoinMessegeSent = true;
        }
    }

}
