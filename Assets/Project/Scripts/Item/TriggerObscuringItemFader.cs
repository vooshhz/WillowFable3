using System.Collections;
using UnityEngine;
using Mirror;

public class TriggerObscuringItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
      
        ObscuringItemFader[] obscuringItemFaders = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        foreach (var fader in obscuringItemFaders)
        {
            fader.FadeOut();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        ObscuringItemFader[] obscuringItemFaders = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        foreach (var fader in obscuringItemFaders)
        {
            fader.FadeIn();
        }
    }
}
