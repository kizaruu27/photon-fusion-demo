using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject sessionItemListPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;

    private void Awake()
    {
        OnLookingForGameSession();
    }

    public void ClearList()
    {
        foreach (Transform child in verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
        
        statusText.gameObject.SetActive(false);
    }

    public void AddToList(SessionInfo sessionInfo)
    {
        SessionInfoListUIItem addedSessionInfoListUIItem = Instantiate(sessionItemListPrefab, verticalLayoutGroup.transform).GetComponent<SessionInfoListUIItem>();
        
        addedSessionInfoListUIItem.SetInformation(sessionInfo);

        addedSessionInfoListUIItem.OnJoinSession += AddedSessionInfoListUIItem_OnJoinSession;
    }

    void AddedSessionInfoListUIItem_OnJoinSession(SessionInfo sessionInfo)
    {
        BasicSpawner networkRunnerHandler = FindObjectOfType<BasicSpawner>();
        networkRunnerHandler.JoinGame(sessionInfo);

        MainMenuHandler mainMenuHandler = FindObjectOfType<MainMenuHandler>();
        mainMenuHandler.OnJoiningServer();
    }

    public void OnNoSessionFound()
    {
        ClearList();
        
        statusText.text = "No game session found";
        statusText.gameObject.SetActive(true);
    }

    public void OnLookingForGameSession()
    {
        ClearList();
        
        statusText.text = "Looking for game session...";
        statusText.gameObject.SetActive(true);
    }
}
