using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class MessegeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] msgWindows;
    Queue msgQ = new Queue();
    
    public void OnMessageReceived(string msg)
    {
        msgQ.Enqueue(msg);

        if(msgQ.Count > 3)
        {
            msgQ.Dequeue();
        }

        int qIndex = 0;
        foreach(string msgText in msgQ)
        {
            msgWindows[qIndex++].text = msgText;
        }
    }
    public void SendMessage(string user, string msg)
    {
        Rpc_InGameMessages("<b>" + user + "</b>" + msg);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_InGameMessages(string msg, RpcInfo info = default)
    {
        OnMessageReceived(msg);
    }
}
