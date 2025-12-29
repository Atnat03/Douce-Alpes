using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;

    [SerializeField] private AudioSO data;
    
    private void Awake()
    {
        instance = this;
    }

    public void PlaySound(int id, float pitch = 1f, float volume = 0.5f)
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(data.audioClips[id], volume);
    }

    public void ButtonClick()
    {
        PlaySound(2);
    }
}
