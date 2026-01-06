using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    private void Awake()
    {
        instance = this;
    }

    public float globalVolume = 1f;
    public bool MusicActivated = true;
    public bool SFXActivated = true;
    public bool VibrationsActivated = true;
    
    [Header("UI")] 
    [SerializeField] private Slider globalVolumeSlider;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Toggle vibrationsToggle;

    public void Start()
    {
        globalVolumeSlider.value = globalVolume;
    }

    public void SetMusicVolume()
    {
        MusicActivated = musicToggle.isOn;
    }

    public void SetSFXVolume()
    {
        SFXActivated = sfxToggle.isOn;
    }
    
    public void SetVibrationsVolume()
    {
        VibrationsActivated = vibrationsToggle.isOn;
    }


    public void OnValueChanged()
    {
        globalVolume = globalVolumeSlider.value;
        AudioListener.volume = globalVolume;
    }
}
