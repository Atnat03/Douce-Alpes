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
    
    public void Start()
    {
        globalVolumeSlider.value = globalVolume;
        musicPlayaToggle.gameObject.SetActive(false);
        
        UpdateMusicVisuals();
    }

    public void SetMusicVolume()
    {
        MusicActivated = musicToggle.isOn;

        if (MusicActivated && isPlayaSound)
        {
            MusicActivated = true;
            isPlayaSound = false;
            musicPlayaToggle.isOn = false;
            DLC.ChangeSelect?.Invoke();
        }

        UpdateMusicVisuals();
    }
    
    private void UpdateMusicVisuals()
    {
        // Cas 1 : musique plage activée
        if (isPlayaSound)
        {
            animatorPlage.SetBool("Sortis", true);
            animatorMontagne.SetBool("Sortis", false);
            return;
        }

        // Cas 2 : musique montagne activée
        if (MusicActivated)
        {
            animatorPlage.SetBool("Sortis", false);
            animatorMontagne.SetBool("Sortis", true);
            return;
        }

        // Cas 3 : aucune musique
        animatorPlage.SetBool("Sortis", false);
        animatorMontagne.SetBool("Sortis", false);
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
        bool wasPlayaActive = isPlayaSound;
        isPlayaSound = musicPlayaToggle.isOn;

        MusicActivated = true;
        
        if (wasPlayaActive && !isPlayaSound)
        {
            musicToggle.isOn = true;
        }
        else if (isPlayaSound)
        {
            musicToggle.isOn = false;
        }

        DLC.ChangeSelect?.Invoke();
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
