using System;
using System.Collections;
using UnityEngine;

public class AmbianceManager : MonoBehaviour
{
    public AudioSource audioSource;
    public float maxVolume = 0.25f;

    private void Start()
    {
        audioSource.Play();
    }
}
