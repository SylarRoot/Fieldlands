using UnityEngine;
using UnityEngine.UI;
using MongoDB.Bson;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TotalStatsManeger : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text durationText;


    public TMP_Text fireEnemiesText;
    public TMP_Text iceEnemiesText;
    public TMP_Text lightningEnemiesText;

    public TMP_Text meleeFireEnemiesText;
    public TMP_Text meleeIceEnemiesText;
    public TMP_Text meleeLightningEnemiesText;

    public TMP_Text projectileFireText;
    public TMP_Text projectileIceText;
    public TMP_Text projectileLightningText;

    public TMP_Text ability2FireText;
    public TMP_Text ability2IceText;
    public TMP_Text ability2LightningText;

    public TMP_Text swordText;


    
    public Button menuButton;


    private EventSystem eventSystem;



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

 

        string username = PlayerPrefs.GetString("LoggedInUser", "Guest");
        usernameText.text = $"Usuari: {username}";

        // Mode Online: Carrega dades de MongoDB
        if (PlayerPrefs.GetInt("IsOnline") == 1 && MongoDBManager.Instance != null)
        {
            BsonDocument stats = MongoDBManager.Instance.GetUserStats(username);
            if (stats != null)
            {
                DisplayStats(stats);
            }
            else
            {
                durationText.text = "No s'han trobat estadístiques per a l'usuari.";
                DisableAllStatTexts();
            }
        }
        // Mode Offline: Missatge informatiu
        else
        {
            durationText.text = "Mode offline: Les estadístiques no es guarden.";
            DisableAllStatTexts();
        }
    }

    void OnMenuButtonClicked()
    {
        
        SceneManager.LoadScene("StartMenu");
    }


    void DisplayStats(BsonDocument stats)
    {
        try
        {
            // Temps total 
            double duration = stats.Contains("totalDuration") ? stats["totalDuration"].AsDouble : 0;

            
            if (duration > 60)
            {
                TimeSpan time = TimeSpan.FromSeconds(duration);
                durationText.text = $"Temps total: {time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
            }
            else
            {
                durationText.text = $"Temps total: {duration:F1}s";
            }

            // Enemics eliminats 
            var enemiesDefeated = stats["enemiesDefeated"].AsBsonDocument;
            fireEnemiesText.text = $"Fire Enemies: {GetValueOrDefault(enemiesDefeated, "Fire")}";
            iceEnemiesText.text = $"Ice Enemies: {GetValueOrDefault(enemiesDefeated, "Ice")}";
            lightningEnemiesText.text = $"Lightning Enemies: {GetValueOrDefault(enemiesDefeated, "Lightning")}";

            meleeFireEnemiesText.text = $"Melee Fire Enemies {GetValueOrDefault(enemiesDefeated, "MeleeFire")}";
            meleeIceEnemiesText.text =  $"Melee Ice Enemies: {GetValueOrDefault(enemiesDefeated, "MeleeIce")}";
            meleeLightningEnemiesText.text = $"Melee Lightning Enemies: {GetValueOrDefault(enemiesDefeated, "MeleeLightning")}";

            // Armes utilitzades 
            var weaponsUsed = stats["weaponsUsed"].AsBsonDocument;
            projectileFireText.text = $"Projectile Fire: {GetValueOrDefault(weaponsUsed, "ProjectileFire")}";
            projectileIceText.text = $"Projectile Ice: {GetValueOrDefault(weaponsUsed, "ProjectileIce")}";
            projectileLightningText.text = $"Projectile Lightning: {GetValueOrDefault(weaponsUsed, "ProjectileLightning")}";

            ability2FireText.text = $"Ability2 Fire: {GetValueOrDefault(weaponsUsed, "Ability2Fire")}";
            ability2IceText.text = $"Ability2 Ice: {GetValueOrDefault(weaponsUsed, "Ability2Ice")}";
            ability2LightningText.text = $"Ability2 Lightning: {GetValueOrDefault(weaponsUsed, "Ability2Lightning")}";

            swordText.text = $"Sword: {GetValueOrDefault(weaponsUsed, "Sword")}";



        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error displaying stats: {ex.Message}");
            durationText.text = "Error al mostrar estadístiques.";
        }
    }

    private int GetValueOrDefault(BsonDocument doc, string key)
    {
        if (doc.Contains(key))
        {
            if (doc[key].IsInt32)
                return doc[key].AsInt32;
            else if (doc[key].IsInt64)
                return (int)doc[key].AsInt64;
            else if (doc[key].IsDouble)
                return (int)doc[key].AsDouble;
        }
        return 0;
    }

    void DisableAllStatTexts()
    {
        fireEnemiesText.gameObject.SetActive(false);
        iceEnemiesText.gameObject.SetActive(false);
        lightningEnemiesText.gameObject.SetActive(false);
        meleeFireEnemiesText.gameObject.SetActive(false);
        meleeIceEnemiesText.gameObject.SetActive(false);
        meleeLightningEnemiesText.gameObject.SetActive(false);
        projectileFireText.gameObject.SetActive(false);
        projectileIceText.gameObject.SetActive(false);
        projectileLightningText.gameObject.SetActive(false);
        ability2FireText.gameObject.SetActive(false);
        ability2IceText.gameObject.SetActive(false);
        ability2LightningText.gameObject.SetActive(false);
        swordText.gameObject.SetActive(false);
    }

}