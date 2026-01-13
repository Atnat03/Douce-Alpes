using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static Settings instance;
    public GameObject buttonSheep;

    private void Awake()
    {
        instance = this;
    }

    public void OpenSettings()
    {
        if (gameManager != null)
        {
            buttonSheep.SetActive(false);
        }
        
        if(SpecialSoundActivated)
            AudioManager.instance.PlaySound(31, 1, 0.25f);
    }

    public void ExitSettings()
    {
        if(gameManager != null)
            buttonSheep.SetActive(true);
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

        musicPlayaToggle.gameObject.SetActive(asDLC);
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

    public void ActivatePlayaToggle()
    {
        asDLC = true;
        musicPlayaToggle.gameObject.SetActive(true);
    } 
    
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

    public SettingsSaveData SaveSettings()
    {
        return new SettingsSaveData(
            globalVolume,
            MusicActivated,
            SFXActivated,
            VibrationsActivated,
            SpecialSoundActivated,
            isPlayaSound,
            asDLC
        );
    }

    public bool asDLC = false;

    public void LoadData(SettingsSaveData data)
    {
        globalVolume = data.globalVolume;
        MusicActivated = data.MusicActivated;
        SFXActivated = data.SFXActivated;
        VibrationsActivated = data.VibrationsActivated;
        SpecialSoundActivated = data.SpecialSoundActivated;
        isPlayaSound = data.isPlayaSound;

        globalVolumeSlider.value = globalVolume;
        AudioListener.volume = globalVolume;
        musicToggle.isOn = MusicActivated;
        sfxToggle.isOn = SFXActivated;
        vibrationsToggle.isOn = VibrationsActivated;
        specialToggle.isOn = SpecialSoundActivated;
        asDLC = data.asDLC;
        musicPlayaToggle.gameObject.SetActive(asDLC);
        musicPlayaToggle.isOn = isPlayaSound;
    }
}

[Serializable]
public class SettingsSaveData
{
    public float globalVolume;
    public bool MusicActivated;
    public bool SFXActivated;
    public bool VibrationsActivated;
    public bool SpecialSoundActivated;
    public bool isPlayaSound;
    public bool asDLC;

    public SettingsSaveData(float globalVolume, bool MusicActivated, bool SFXActivated, bool VibrationsActivated,
        bool SpecialSoundActivated, bool isPlayaSound, bool asDLC)
    {
        this.globalVolume = globalVolume;
        this.MusicActivated = MusicActivated;
        this.SFXActivated = SFXActivated;
        this.VibrationsActivated = VibrationsActivated;
        this.SpecialSoundActivated = SpecialSoundActivated;
        this.isPlayaSound = true;
        this.asDLC = asDLC;
    }
}
