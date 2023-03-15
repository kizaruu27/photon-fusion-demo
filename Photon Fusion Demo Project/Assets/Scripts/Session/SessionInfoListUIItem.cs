using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionInfoListUIItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sessionNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Button joinButton;

    private SessionInfo sessionInfo;
    
    // unity events
    public event Action<SessionInfo> OnJoinSession;

    public void SetInformation(SessionInfo sessionInfo)
    {
        this.sessionInfo = sessionInfo;

        sessionNameText.text = sessionInfo.Name;
        playerCountText.text = $"{sessionInfo.PlayerCount.ToString()} / {sessionInfo.MaxPlayers.ToString()}";

        bool isJoinButtonActive = true;

        if (sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
        {
            isJoinButtonActive = false;
        }
        
        joinButton.gameObject.SetActive(isJoinButtonActive);
    }

    public void OnClick()
    {
        OnJoinSession?.Invoke(sessionInfo);
    }
}
