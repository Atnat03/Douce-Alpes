using UnityEngine;

public class PlayFootSteps : MonoBehaviour
{
    public AudioSource audioManager;
    public AudioClip audioClip;
    
    public void PlayFootSound()
    {
        audioManager.pitch = Random.Range(0.8f, 1.2f);
        audioManager.PlayOneShot(audioClip);
    }
}
