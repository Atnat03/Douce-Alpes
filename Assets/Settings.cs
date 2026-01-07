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

    public void OpenSettings()
    {
        if(SpecialSoundActivated)
            AudioManager.instance.PlaySound(31, 1, 0.25f);
    }
    
    public float globalVolume = 1f;
    public bool MusicActivated = true;
    public bool SFXActivated = true;
    public bool VibrationsActivated = true;
    public bool SpecialSoundActivated = false;
    
    [Header("UI")] 
    [SerializeField] private Slider globalVolumeSlider;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Toggle vibrationsToggle;
    [SerializeField] private Toggle specialToggle;

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

    public void SetSpecialVolume()
    {
        SpecialSoundActivated = specialToggle.isOn;
    }

    public void OnValueChanged()
    {
        globalVolume = globalVolumeSlider.value;
        AudioListener.volume = globalVolume;
    }
}
