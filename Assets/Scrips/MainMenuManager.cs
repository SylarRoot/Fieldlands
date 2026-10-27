using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using MongoDB.Bson;
using System;

public class MainMenuManager : MonoBehaviour
{
    // Referències UI
    public Button offlineButton;
    public Button TotalStatsButton;
    public Button StartButton;
    public Button UserButton;
    public Button ExitButton;



    void Start()
    {
        // Assigna els listeners als botons

        offlineButton.onClick.AddListener(OnOfflineClicked);
        TotalStatsButton.onClick.AddListener(OnTotalStatsClicked);
        StartButton.onClick.AddListener(OnStartClicked);
        UserButton.onClick.AddListener(OnUserClicked);
        ExitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    

    public void OnTotalStatsClicked()
    {
        SceneManager.LoadScene("TotalStats");
    }
    public void OnOfflineClicked()
    {
        PlayerPrefs.SetString("LoggedInUser", "Guest"); // Usuari offline
        PlayerPrefs.SetInt("IsOnline", 0); // Mode offline
        PlayerPrefs.Save();
        SceneManager.LoadScene("Main_Scene");
    }

    public void OnStartClicked()
    {
        SceneManager.LoadScene("Main_Scene");
    }


    public void OnUserClicked()
    {
        SceneManager.LoadScene("Login_Menu");
    }


    void OnQuitButtonClicked()
    {
        
        Application.Quit();
    }
}