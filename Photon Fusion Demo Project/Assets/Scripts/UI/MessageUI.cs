using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class MessageUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI[] msgWindows;
    Queue msgQ = new Queue();

    // Start is called before the first frame update
    void Start()
    {
        
    }

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
