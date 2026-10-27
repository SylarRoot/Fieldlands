using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public TMP_Text errorText;
    public TMP_Text SuccessText;

    public Button loginButton;
    public Button registerButton;

    public Button backToMenuButton;




    void Start()
    {

        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);
        backToMenuButton.onClick.AddListener(OnBackToMenuClicked);

    

    }


    // Botó "Iniciar Sessió"
    private void OnLoginClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorText.text = "Error: Cal introduir usuari i contrasenya.";
            return;
        }

        if (MongoDBManager.Instance.AuthenticateUser(username, password))
        {
            errorText.text = "";
            SuccessText.text = "Autenticació exitosa!";
            PlayerPrefs.SetString("LoggedInUser", username);
            PlayerPrefs.SetInt("IsOnline", 1); // Mode online
            PlayerPrefs.Save();
            SceneManager.LoadScene("StartMenu");

        }
        else
        {
            errorText.text = "Error: Usuari o contrasenya incorrectes.";
        }
    }

    // Botó "Registrar-se"
    public void OnRegisterClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorText.text = "Error: Cal introduir usuari i contrasenya.";
            return;
        }

        if (MongoDBManager.Instance.RegisterUser(username, password))
        {
            SuccessText.text = "Registre exitós! Ara pots iniciar sessió.";
            Debug.Log("Usuari registrat correctament!");
        }
        else
        {
            errorText.text = "Error: L'usuari ja existeix o hi ha hagut un problema.";
        }
    }

    public void OnBackToMenuClicked()
    {
        SceneManager.LoadScene("StartMenu");
    }

}
