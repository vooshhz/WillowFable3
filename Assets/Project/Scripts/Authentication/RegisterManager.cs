using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TextMeshProUGUI messageText;
    public Button registerButton;

    private FirebaseAuth auth;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;                
            }
            else
            {
                messageText.text = "Firebase initialization failed: " + task.Result.ToString();
            }
        });
    }

    public void RegisterNewUser()
    {
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Email and Password cannot be empty.";
            return;
        }

        messageText.text = "Registering user...";

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                messageText.text = "Registration canceled.";
                Debug.LogError("Registration canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                messageText.text = "Registration failed.";
                Debug.LogError("Registration failed.");
                foreach (var exception in task.Exception.Flatten().InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        Debug.LogError($"Firebase Registration Error Code: {errorCode}");
                        messageText.text = $"Registration failed: {errorCode.ToString()} - {firebaseEx.Message}";
                    }
                    else
                    {
                        messageText.text = "Registration failed: " + exception.Message;
                    }
                }
                return;
            }

            FirebaseUser newUser = task.Result.User;
            messageText.text = "Registration successful! You can now log in.";
            Debug.Log($"Registration successful! User: {newUser.Email}, UID: {newUser.UserId}");

            // OPTIONAL: Sign out immediately to prevent auto-login effect
            auth.SignOut(); // <-- THIS LINE LOGS THEM OUT RIGHT AFTER REGISTRATION
        });
    }
}
