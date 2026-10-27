using UnityEngine;
using UnityEngine.UI;
using MongoDB.Bson;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using MongoDB.Bson.Serialization.Serializers;

public class GameOverManager : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text durationText;

    public TMP_Text fireEnemiesText;
    public TMP_Text iceEnemiesText;
    public TMP_Text lightningEnemiesText;

    public TMP_Text meleeFireEnemiesText;
    public TMP_Text meleeIceEnemiesText;
    public TMP_Text meleeLightningEnemiesText;

    public TMP_Text swordText;

    public TMP_Text projectileFireText;
    public TMP_Text projectileIceText;
    public TMP_Text projectileLightningText;

    public TMP_Text ability2FireText;
    public TMP_Text ability2IceText;
    public TMP_Text ability2LightningText;

    
    public Button menuButton;
    public Button retryButton;
    public Button quitButton;
    public Button TotalStats;



    private EventSystem eventSystem;




    private int currentFireEnemies;
    private int currentIceEnemies;
    private int currentLightningEnemies;

    private int currentMeleeFireEnemies;
    private int currentMeleeIceEnemies;
    private int currentMeleeLightningEnemies;

    private int currentProjectileFire;
    private int currentProjectileIce;
    private int currentProjectileLightning;

    private int currentAbility2Fire;
    private int currentAbility2Ice;
    private int currentAbility2Lightning;

    private int currentSword;


    private double currentSessionDuration;


    void Awake()
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("EventSystem creat automàticament");
        }
    }

    void Start()
    {
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuButtonClicked);
            Debug.Log("Menu button listener added");
        }
        else
        {
            Debug.LogError("Menu button reference missing");
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryButtonClicked);
            Debug.Log("Retry button listener added");
        }
        else
        {
            Debug.LogError("Retry button reference missing");
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
            Debug.Log("Quit button listener added");
        }
        else
        {
            Debug.LogError("Quit button reference missing");
        }

        if (TotalStats != null)
        {
            TotalStats.onClick.AddListener(OnTotalStatsButtonClicked);
            Debug.Log("TotalStats button listener added");
        }
        else
        {
            Debug.LogError("TotalStats button reference missing");
        }



        // Carrega les estadístiques de la partida actual des de PlayerPrefs
        currentSessionDuration = PlayerPrefs.GetFloat("CurrentSessionDuration");

        currentFireEnemies = PlayerPrefs.GetInt("CurrentFireEnemies");
        currentIceEnemies = PlayerPrefs.GetInt("CurrentIceEnemies");
        currentLightningEnemies = PlayerPrefs.GetInt("CurrentLightningEnemies");

        currentMeleeFireEnemies = PlayerPrefs.GetInt("CurrentMeleeFireEnemies");
        currentMeleeIceEnemies = PlayerPrefs.GetInt("CurrentMeleeIceEnemies");
        currentMeleeLightningEnemies = PlayerPrefs.GetInt("CurrentMeleeLightningEnemies");

        currentProjectileFire = PlayerPrefs.GetInt("CurrentProjectileFire");
        currentProjectileIce = PlayerPrefs.GetInt("CurrentProjectileIce");
        currentProjectileLightning = PlayerPrefs.GetInt("CurrentProjectileLightning");

        currentAbility2Fire = PlayerPrefs.GetInt("CurrentAbility2Fire");
        currentAbility2Ice = PlayerPrefs.GetInt("CurrentAbility2Ice");
        currentAbility2Lightning = PlayerPrefs.GetInt("CurrentAbility2Lightning");

        currentSword = PlayerPrefs.GetInt("CurrentSword");


        DisplayCurrentGameStats();

    }

    void OnTotalStatsButtonClicked()
    {
        SceneManager.LoadScene("TotalStats");
        Reset();
    }
    void OnMenuButtonClicked()
    {
        SceneManager.LoadScene("StartMenu");
        Reset();
    }

    void OnRetryButtonClicked()
    {
        SceneManager.LoadScene("Main_Scene");
        Reset();
    }

    void OnQuitButtonClicked()
    {
        // Quit the application
        Application.Quit();
        Reset();
    }



    void DisplayCurrentGameStats()
    {
        string username = PlayerPrefs.GetString("LoggedInUser", "Guest");
        usernameText.text = $"Username: {username}"; 
        durationText.text = $"Temps de la partida: {currentSessionDuration:F1}s";

        fireEnemiesText.text = $"Fire Enemies: {currentFireEnemies}";
        iceEnemiesText.text = $"Ice Enemies : {currentIceEnemies}";
        lightningEnemiesText.text = $"LightningEnemies: {currentLightningEnemies}";

        meleeFireEnemiesText.text = $"Melee Fire Enemies: {currentMeleeFireEnemies} ";
        meleeIceEnemiesText.text = $"Melee Ice Enemies: {currentMeleeIceEnemies} ";
        meleeLightningEnemiesText.text = $"Melee Lightning Enemies: {currentMeleeLightningEnemies} ";

        projectileFireText.text = $"Projectile Fire: {currentProjectileFire} ";
        projectileIceText.text = $"Projectile Ice: {currentProjectileIce} ";
        projectileLightningText.text = $"Projectile Lightning: {currentProjectileLightning} ";

        ability2FireText.text = $"Ability2 Fire : {currentAbility2Fire} ";
        ability2IceText.text = $"Ability2 Ice : {currentAbility2Ice} ";
        ability2LightningText.text = $"Ability2 Lightning : {currentAbility2Lightning} ";
        swordText.text = $"Sword : {currentSword} ";

       
    }

    void Reset()
    {
        // Reset all stats to zero
        PlayerPrefs.SetFloat("CurrentSessionDuration", 0f);
        PlayerPrefs.SetInt("CurrentFireEnemies", 0);
        PlayerPrefs.SetInt("CurrentIceEnemies", 0);
        PlayerPrefs.SetInt("CurrentLightningEnemies", 0);
        PlayerPrefs.SetInt("CurrentMeleeFireEnemies", 0);
        PlayerPrefs.SetInt("CurrentMeleeIceEnemies", 0);
        PlayerPrefs.SetInt("CurrentMeleeLightningEnemies", 0);
        PlayerPrefs.SetInt("CurrentProjectileFire", 0);
        PlayerPrefs.SetInt("CurrentProjectileIce", 0);
        PlayerPrefs.SetInt("CurrentProjectileLightning", 0);
        PlayerPrefs.SetInt("CurrentAbility2Fire", 0);
        PlayerPrefs.SetInt("CurrentAbility2Ice", 0);
        PlayerPrefs.SetInt("CurrentAbility2Lightning", 0);
        PlayerPrefs.SetInt("CurrentSword", 0);
    }




}