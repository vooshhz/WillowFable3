using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateCharacterButton : MonoBehaviour
{
    public void LoadCreateCharacterScene()
    {
        SceneManager.LoadScene("Scene_CreateCharacter");
    }
}