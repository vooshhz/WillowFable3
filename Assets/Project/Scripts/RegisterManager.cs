using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

public class RegisterUser : MonoBehaviour
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
                messageText.text = "Firebase Initialized Successfully";
                registerButton.onClick.AddListener(RegisterNewUser);
            }
            else
            {
                messageText.text = "Firebase initialization failed: " + task.Result.ToString();
            }
        });
    }

    private void RegisterNewUser()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Email and Password cannot be empty.";
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
                foreach (var exception in task.Exception.Flatten().InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        switch ((AuthError)firebaseEx.ErrorCode)
                        {
                            case AuthError.EmailAlreadyInUse:
                                messageText.text = "Email is already in use.";
                                break;
                            case AuthError.InvalidEmail:
                                messageText.text = "Invalid email address.";
                                break;
                            case AuthError.WeakPassword:
                                messageText.text = "Weak password, choose a stronger one.";
                                break;
                            default:
                                messageText.text = "Registration failed: " + firebaseEx.Message;
                                break;
                        }
                    }
                }
                return;
            }

            FirebaseUser newUser = task.Result.User;
            messageText.text = "Registration successful! Welcome, " + newUser.Email;
        });
    }
}
