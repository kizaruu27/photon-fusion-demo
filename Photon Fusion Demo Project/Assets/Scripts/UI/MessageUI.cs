using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.EventSystems;

public class MessageUI : NetworkBehaviour, IChatClientListener, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI[] msgWindows;
    Queue msgQ = new Queue();
    Queue chatQ = new Queue();
    public TMP_InputField chatBox;
    private bool chat;
    [SerializeField] private TextMeshProUGUI sysBtnTxt, chatBtnTxt;
    public ChatClient chatClient;

    // Start is called before the first frame update
    void Start()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect("5487fd16-6b08-4626-abf5-2334ad4abec3", "2.17", new AuthenticationValues(PlayerDataContainer.playerName));
    }


    public override void FixedUpdateNetwork()
    {
        try
        {
            chatClient.Service();
        }
        catch
        {

        }

        if(chatBox.text != "" && Input.GetKey(KeyCode.Return))
        {
            SendChat();
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(chat)
            {
                ShowSystem();
            }
            else
            {
                ShowChat();
            }
        }
    }

    public void OnMessageReceived(string msg)
    {
        if (msg.Contains(':'))
        {
            chatQ.Enqueue(msg);

            if (chatQ.Count > 3)
            {
                chatQ.Dequeue();
            }
        }
        else
        {
            msgQ.Enqueue(msg);

            if (msgQ.Count > 3)
            {
                msgQ.Dequeue();
            }
        }

        UpdateMessage();
    }

    private void UpdateMessage()
    {
        int qIndex = 0;

        while (qIndex < msgWindows.Length)
        {
            msgWindows[qIndex++].text = "";
        }

        qIndex = 0;
        if (chat && chatQ.Count > 0)
        {
            foreach (string msgText in chatQ)
            {
                msgWindows[qIndex++].text = msgText;
            }
        }
        else if(!chat && msgQ.Count > 0)
        {
            foreach (string msgText in msgQ)
            {
                msgWindows[qIndex++].text = msgText;
            }
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

    public void SendChat()
    {
        if(chatBox.text != "")
        {
            string user = PlayerDataContainer.playerName;
            string msg = chatBox.text;
            chatBox.text = "";
            chatClient.PublishMessage("Public", msg);
            //Rpc_InGameMessages("<b>" + user + ":</b> " + msg);
        }
    }
    public void ShowChat()
    {
        chat = true;
        chatBox.gameObject.SetActive(true);
        sysBtnTxt.text = "Show System";
        chatBtnTxt.text = "Chat";
        UpdateMessage();
    }

    public void ShowSystem()
    {
        chat = false;
        chatBox.gameObject.SetActive(false);
        sysBtnTxt.text = "System";
        chatBtnTxt.text = "Show Chat";
        UpdateMessage();
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDisconnected()
    {
        //throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        chatClient.Subscribe(new string[] { "Public" });
    }

    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //string msgs = "";

        //for(int i = senders.Length; i > senders.Length - 3; i++)
        //{
            //msgs = senders[i] + ": " + messages[i];

            //msgWindows[3-i].text = msgs;
        //}

        OnMessageReceived(senders[senders.Length-1] + ": " + messages[messages.Length-1]);
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        chatClient.PublishMessage("Public", PlayerDataContainer.playerName + " has joined the chat");
        //throw new System.NotImplementedException();
    }

    public void OnUnsubscribed(string[] channels)
    {
        chatClient.PublishMessage("Public", PlayerDataContainer.playerName + " has left the chat");
        //throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.parent.gameObject.GetComponent<PlayerController>().isChatting = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.parent.gameObject.GetComponent<PlayerController>().isChatting = false;
    }
}
