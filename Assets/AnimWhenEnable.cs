using System;
using UnityEngine;
using UnityEngine.UI;

public class AnimWhenEnable : MonoBehaviour
{
    private void Update()
    {
        GetComponent<Animator>().enabled = GetComponent<Button>().interactable;
    }
}
