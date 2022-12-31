using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hoverAudioClip;
    public AudioClip clickAudioClip;

    public void hoverSound()
    {
        audioSource.PlayOneShot(hoverAudioClip);
    }

    public void clickSound()
    {
        audioSource.PlayOneShot(clickAudioClip);
    }
}
