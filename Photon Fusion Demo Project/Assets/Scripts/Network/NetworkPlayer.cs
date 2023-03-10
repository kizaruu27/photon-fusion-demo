using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    [SerializeField] private Transform playerCamera;
    public TextMeshPro uName;

    public static NetworkPlayer Local { get; set; }

    [Networked(OnChanged = nameof(SetUserName))]
    public NetworkString<_16> userName { get; set; }
    bool joinMessageSent = false;

    [SerializeField] private MessageUI msgUI;

    private void Awake()
    {
        Local = this;
        msgUI = GameObject.Find("Messages").GetComponent<MessageUI>();
    }

    public override void Spawned()
    {
        RPC_SetUserName(PlayerPrefs.GetString("PlayerUserName"));
        if (!Object.HasInputAuthority)
        {
            playerCamera.gameObject.SetActive(false);
            //GetComponent<PlayerController>().enabled = false;
            //RPC_SetUserName(PlayerPrefs.GetString("PlayerUserName"));
            //uName.text = userName.ToString();
        }
        else
        {
            RPC_SetUserName(PlayerPrefs.GetString("PlayerUserName"));
            uName.text = userName.ToString();

            transform.GetChild(0).gameObject.SetActive(false);
            playerCamera.parent = null;
            
            Camera mainCamera = Camera.main;
            mainCamera.enabled = false;

            AudioListener audioListener = FindObjectOfType<AudioListener>();
            audioListener.enabled = false;
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if(Object.HasStateAuthority)
        {
            if(Runner.TryGetPlayerObject(player, out NetworkObject playerLeftNetworkObject))
            {
                Local.msgUI.SendMessage(playerLeftNetworkObject.GetComponent<NetworkPlayer>().userName.ToString(), "left the session");
            }
        }
        if(player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    private void SetUserName()
    {
        Debug.Log("Nickname Set for player " + gameObject.name);

        uName.text = userName.ToString();
    }
    static void SetUserName(Changed<NetworkPlayer> changed)
    {
        Debug.Log("On " + Time.time + " changed " + changed.Behaviour.userName);

        changed.Behaviour.SetUserName();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetUserName(string userName, RpcInfo info = default)
    {
        Debug.Log("[RPC] SetUserName " + userName);
        this.userName = userName;

        if(!joinMessageSent)
        {
            msgUI.SendMessage(userName.ToString(), " joined the session");
            joinMessageSent = true;
        }
    }
}
