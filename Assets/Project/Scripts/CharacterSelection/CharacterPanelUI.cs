using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterPanelUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text classText;
    public TMP_Text levelText;
    public RawImage rawImage; // This is your panel's portrait display

    public RenderTexture renderTexture1;
    public RenderTexture renderTexture2;
    public RenderTexture renderTexture3;

    public void SetCharacterInfo(string charName, string charClass, int charLevel)
    {
        if (nameText != null) nameText.text = charName;
        if (classText != null) classText.text = charClass;
        if (levelText != null) levelText.text = "Level " + charLevel;
    }

    public void SetPortrait(int characterIndex)
    {
        if (rawImage == null) return;

        switch (characterIndex)
        {
            case 1:
                rawImage.texture = renderTexture1;
                break;
            case 2:
                rawImage.texture = renderTexture2;
                break;
            case 3:
                rawImage.texture = renderTexture3;
                break;
            default:
                Debug.LogWarning("Invalid character index: " + characterIndex);
                break;
        }
    }
}
