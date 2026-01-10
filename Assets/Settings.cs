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

    public Animator animatorPlage;
    public Animator animatorMontagne;
    
    public GameManager gameManager;
    
    private void Start()
    {
        globalVolumeSlider.value = globalVolume;

        musicToggle.isOn = MusicActivated;
        musicPlayaToggle.isOn = isPlayaSound;
        
        SetPlayaSound(false);
        
        if (MusicActivated)
            DLC.ChangeSelect?.Invoke();

        UpdateMusicVisuals();
    }

    public void SetMusicVolume()
    {
        MusicActivated = musicToggle.isOn;

        if (MusicActivated)
        {
            isPlayaSound = false;
            musicPlayaToggle.isOn = false;

            DLC.ChangeSelect?.Invoke();
        }

        UpdateMusicVisuals();
    }



    private void UpdateMusicVisuals()
    {
        if (!MusicActivated)
        {
            animatorPlage.SetBool("Sortis", false);
            animatorMontagne.SetBool("Sortis", false);
            return;
        }

        animatorPlage.SetBool("Sortis", isPlayaSound);
        animatorMontagne.SetBool("Sortis", !isPlayaSound);
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

        if (isPlayaSound)
        {
            MusicActivated = true;
            musicToggle.isOn = true;

            DLC.ChangeSelect?.Invoke();
        }
        else
        {
            // On revient à montagne si musique activée
            if (MusicActivated)
                DLC.ChangeSelect?.Invoke();
        }

        UpdateMusicVisuals();
    }



    public void SetPlayaSound(bool state)
    {
        isPlayaSound = state;
        musicPlayaToggle.isOn = state;
        UpdateMusicVisuals();
    }

    public void OnValueChanged()
    {
        globalVolume = globalVolumeSlider.value;
        AudioListener.volume = globalVolume;
    }
}
