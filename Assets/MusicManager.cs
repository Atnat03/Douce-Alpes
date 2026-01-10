using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip dehorsMusic;
    public AudioClip dedansMusic;
    public AudioClip playaMusic;
    public bool isDehors = true;

    public float maxVolume = 0.5f;
    public float fadeDuration = 0.25f;

    private Coroutine fadeCoroutine;

    [SerializeField] private DLC dlc;
    
    private void Start()
    {
        audioSource.clip = dehorsMusic;
        audioSource.volume = maxVolume;
        audioSource.Play();
    }

    public void ChangeMusique()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(ChangeMusicCoroutine());
    }

    IEnumerator ChangeMusicCoroutine()
    {
        yield return StartCoroutine(FadeVolume(audioSource.volume, 0f));

        audioSource.Stop();
        isDehors = !isDehors;
        audioSource.pitch = 1f;
        audioSource.clip = isDehors ? dehorsMusic : dedansMusic;

        if (isDehors && GameData.instance.timer.currentMiniJeuToDo == MiniGames.Sortie)
        {
            audioSource.pitch = 0.85f;
        }

        if (Settings.instance.isPlayaSound)
        {
            audioSource.clip = playaMusic;
        }
        
        audioSource.Play();
        
        yield return new WaitForSeconds(fadeDuration*2);

        yield return StartCoroutine(FadeVolume(0f, maxVolume));
    }

    IEnumerator FadeVolume(float startVolume, float endVolume)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = endVolume;
    }

    private void Update()
    {
        audioSource.volume = Settings.instance.MusicActivated ? maxVolume : 0f;
    }

    void OnEnable()
    {
        DLC.ChangeSelect += ChangeMusique;
    }
}