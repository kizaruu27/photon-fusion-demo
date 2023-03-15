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
        // if (PlayerPrefs.HasKey("PlayerNickname"))
        //     nicknameField.text = PlayerPrefs.GetString("PlayerNickname");
    }

    public void OnFindGameClickede()
    {
        PlayerDataContainer.playerName = nicknameField.text;
        
        // PlayerPrefs.SetString("PlayerNickname", nicknameField.text);
        // PlayerPrefs.Save();

        // SceneManager.LoadScene("Gameplay");

        HideAllPanels();

        sessionBrowserPanel.gameObject.SetActive(true);
        FindObjectOfType<SessionListUIHandler>(true);
        
        BasicSpawner networkRunnerHandler = FindObjectOfType<BasicSpawner>();
        networkRunnerHandler.OnJoinLobby();
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
        StartCoroutine(StartNewSession());
    }

    IEnumerator StartNewSession()
    {
        HideAllPanels();
        statusPanel.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);

        BasicSpawner networkRunnerHandler = FindObjectOfType<BasicSpawner>();
        networkRunnerHandler.CreateGame(sessionNameInputField.text, SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnJoiningServer()
    {
        HideAllPanels();
        statusPanel.gameObject.SetActive(true);
    }
}
