using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkInGameMessege : NetworkBehaviour
{
    [SerializeField] private InGameMessegeHandler inGameMessegeHandler;

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_InGameMessege(string messege, RpcInfo info = default)
    {
        Debug.Log($"[RPC] InGameMessege {messege}");

        if (inGameMessegeHandler == null)
            inGameMessegeHandler = NetworkPlayer.Local.localCamera.GetComponentInChildren<InGameMessegeHandler>();
        
        if (inGameMessegeHandler != null)
            inGameMessegeHandler.OnGameMessegeReceived(messege);
    }

    public void SendInGameRPCMessege(string userNickname, string messege)
    {
        RPC_InGameMessege($"{userNickname} {messege}");
    }
}
