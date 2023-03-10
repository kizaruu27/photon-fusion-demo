using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        inputField.characterLimit = 16;
        if (PlayerPrefs.HasKey("PlayerUserName"))
        {
            inputField.text = PlayerPrefs.GetString("PlayerUserName");
            //SceneManager.LoadScene("Gameplay");
        }
    }

    public void StartGame()
    {
        PlayerPrefs.SetString("PlayerUserName", inputField.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Gameplay");
    }
}
