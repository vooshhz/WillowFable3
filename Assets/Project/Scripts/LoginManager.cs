using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Extensions;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;
    public Button loginButton;
    public Button registerButton;

    private FirebaseAuth auth;
    private FirebaseUser user;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                messageText.text = "Firebase Initialized";
            }
            else
            {
                messageText.text = "Firebase failed to initialize: " + task.Result.ToString();
            }
        });

        loginButton.onClick.AddListener(LoginUser);
        registerButton.onClick.AddListener(RegisterUser);
    }

    public void RegisterUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Email and password cannot be empty.";
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                messageText.text = "Registration canceled.";
                return;
            }
            if (task.IsFaulted)
            {
                messageText.text = "Registration failed: " + task.Exception?.GetBaseException().Message;
                return;
            }

            AuthResult result = task.Result;
            user = result.User;

            messageText.text = "Registration successful! Welcome, " + user.Email;
        });
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
                return;
            }
            if (task.IsFaulted)
            {
                messageText.text = "Login failed: " + task.Exception?.GetBaseException().Message;
                return;
            }

            AuthResult result = task.Result;
            user = result.User;

            messageText.text = "Login successful! Welcome back, " + user.Email;
            // Here you can load the next scene or enable the next part of your game
        });
    }
}
