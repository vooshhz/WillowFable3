using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TMP_Text messageText;
    private FirebaseAuth auth;
    private FirebaseUser user;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // Set the Database URL here
                app.Options.DatabaseUrl = new System.Uri("https://willowfable3-default-rtdb.firebaseio.com/");

                Debug.Log("Firebase Project ID: " + app.Options.ProjectId);
                Debug.Log("Firebase Database URL: " + app.Options.DatabaseUrl);
                Debug.Log("Firebase API Key: " + app.Options.ApiKey);
                Debug.Log("Firebase App ID: " + app.Options.AppId);

                auth = FirebaseAuth.DefaultInstance;
                messageText.text = "Firebase Initialized";
                Debug.Log("Authentication connected to Firebase.");
            }
            else
            {
                messageText.text = "Firebase failed to initialize: " + task.Result.ToString();
                Debug.LogError("Firebase failed to initialize: " + task.Result.ToString());
            }
        });

        loginButton.onClick.AddListener(LoginUser);
    }

    public void LoginUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Email and password cannot be empty.";
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                messageText.text = "Login canceled.";
                Debug.LogError("Login canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                FirebaseException firebaseEx = task.Exception?.Flatten().InnerExceptions[0] as FirebaseException;
                AuthError errorCode = firebaseEx != null ? (AuthError)firebaseEx.ErrorCode : AuthError.None;

                Debug.LogError($"Login Failed: {firebaseEx?.Message}");
                Debug.LogError($"Firebase Error Code: {errorCode}");

                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        messageText.text = "Email is required.";
                        break;
                    case AuthError.MissingPassword:
                        messageText.text = "Password is required.";
                        break;
                    case AuthError.InvalidEmail:
                        messageText.text = "Invalid email format.";
                        break;
                    case AuthError.UserNotFound:
                        messageText.text = "Account not found.";
                        break;
                    case AuthError.WrongPassword:
                        messageText.text = "Incorrect password.";
                        break;
                    default:
                        messageText.text = "Login failed: " + firebaseEx?.Message;
                        break;
                }

                // Special Unity Editor workaround for internal error
                if (Application.isEditor && firebaseEx != null && firebaseEx.Message.Contains("internal error"))
                {
                    messageText.text = "Account not found (Editor Workaround)";
                }

                return;
            }

            // Success
            user = task.Result.User;
            messageText.text = "Login successful! Welcome back, " + user.Email;
            Debug.Log("Login successful: " + user.Email);

            // Start coroutine to delay scene load by 1 second
            StartCoroutine(LoadCharacterSelectionScene());
        });
    }

    private IEnumerator LoadCharacterSelectionScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Scene_CharacterSelection");
    }


}
