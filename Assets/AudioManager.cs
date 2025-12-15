using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;
    
    private void Awake()
    {
        instance = this;
    }

    public void PlaySound(AudioClip clip, float pitch = 1f, float volume = 0.5f)
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, volume);
    }
}
