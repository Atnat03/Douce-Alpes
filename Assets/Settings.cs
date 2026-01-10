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
        if(gameManager != null)
            gameManager.ChangeCameraState(CamState.UI);
        
        if(SpecialSoundActivated)
            AudioManager.instance.PlaySound(31, 1, 0.25f);
    }

    public void ExitSettings()
    {
        if(gameManager != null)
            gameManager.ChangeCameraState(CamState.Default);
    }
    
    public float globalVolume = 1f;
    public bool MusicActivated = true;
    public bool SFXActivated = true;
    public bool VibrationsActivated = true;
    public bool SpecialSoundActivated = false;
    public bool isPlayaSound = false;
    
    [Header("UI")] 
    [SerializeField] private Slider globalVolumeSlider;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Toggle vibrationsToggle;
    [SerializeField] private Toggle specialToggle;
    [SerializeField] private Toggle musicPlayaToggle;
    
    public GameManager gameManager;
    
    private void Start()
    {
        globalVolumeSlider.value = globalVolume;
        AudioListener.volume = globalVolume;

        musicToggle.isOn = MusicActivated;
        musicPlayaToggle.isOn = isPlayaSound;

        musicPlayaToggle.gameObject.SetActive(false);
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

    public void ActivatePlayaToggle() => musicPlayaToggle.gameObject.SetActive(true);
    
    public void SetPlayaSound()
    {
        isPlayaSound = musicPlayaToggle.isOn;
        DLC.ChangeSelect?.Invoke();
    }

    public void SetPlayaSound(bool state)
    {
        isPlayaSound = state;
        musicPlayaToggle.isOn = state;
    }

    public void OnValueChanged()
    {
        globalVolume = globalVolumeSlider.value;
        AudioListener.volume = globalVolume;
    }
}
