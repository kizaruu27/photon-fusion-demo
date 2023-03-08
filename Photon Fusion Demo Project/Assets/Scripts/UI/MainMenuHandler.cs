using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [Header("Panels")]
    public GameObject playerDetailsPanel;
    public GameObject sessionBrowserPanel;
    public GameObject createSessionPanel;
    public GameObject statusPanel;
    
    [Header("Player Settings")]
    [SerializeField] private TMP_InputField nicknameField;
    
    [Header("New Game Session")]
    public TMP_InputField sessionNameInputField;
    

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
            nicknameField.text = PlayerPrefs.GetString("PlayerNickname");
    }

    public void OnFindGameClickede()
    {
        PlayerPrefs.SetString("PlayerNickname", nicknameField.text);
        PlayerPrefs.Save();

        // SceneManager.LoadScene("Gameplay");
        
        BasicSpawner networkRunnerHandler = FindObjectOfType<BasicSpawner>();
        networkRunnerHandler.OnJoinLobby();
            
        HideAllPanels();

        sessionBrowserPanel.gameObject.SetActive(true);
    }
    
    void HideAllPanels()
    {
        playerDetailsPanel.SetActive(false);
        sessionBrowserPanel.SetActive(false);
        createSessionPanel.SetActive(false);
        statusPanel.SetActive(false);
    }
    
    public void OnCreateNewGameClicked()
    {
        HideAllPanels();
	
        createSessionPanel.SetActive(true);
    }

    public void OnStartNewSessionClicked()
    {
        BasicSpawner networkRunnerHandler = FindObjectOfType<BasicSpawner>();
        networkRunnerHandler.CreateGame(sessionNameInputField.text, "Gameplay");
        
        HideAllPanels();
        statusPanel.gameObject.SetActive(true);
    }
}
