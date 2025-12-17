using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoCailloux : MonoBehaviour
{
    public List<int> correctMelody = new();
    public List<int> currentTry = new();
    
    public AudioSource audioSource;

    public AudioClip badMelody;
    public AudioClip goodMelody;

    private bool canTouch = true;

    public void PlayTouche(AudioClip clip, int id)
    {
        if (!canTouch)
            return;
        
        audioSource.PlayOneShot(clip);
        currentTry.Add(id);

        if (currentTry.Count == correctMelody.Count)
        {
            if (CheckIfCorrectMelody())
            {
                StartCoroutine(WaitBeforeBackFlip());
                Debug.Log("Good melody");
            }
            else
            {
                audioSource.PlayOneShot(badMelody);
                Debug.Log("Bad melody");
            }
            currentTry =  new List<int>();
        }
        
    }

    IEnumerator WaitBeforeBackFlip()
    {
		yield return new WaitForSeconds(0.25f);
        canTouch = false;
        audioSource.PlayOneShot(goodMelody);
        yield return new WaitForSeconds(0.5f);

        GameManager.instance.AnimatedBackFlip();
        canTouch = true;
    }

    public bool CheckIfCorrectMelody()
    {
        for (int i = 0; i < correctMelody.Count; i++)
        {
            if(correctMelody[i] != currentTry[i])
                return false;
        }

        return true;
    }
}
