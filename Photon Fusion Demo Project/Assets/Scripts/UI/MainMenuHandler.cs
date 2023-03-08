using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField nicknameField;

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
            nicknameField.text = PlayerPrefs.GetString("PlayerNickname");
    }

    public void OnClickJoinGame()
    {
        PlayerPrefs.SetString("PlayerNickname", nicknameField.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Gameplay");
    }
}
