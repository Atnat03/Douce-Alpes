using System;
using UnityEngine;

public class ActivateStartUI : MonoBehaviour
{
    [SerializeField] private GameObject[] uis;

    private void Start()
    {
        foreach (GameObject g in uis)
        {
            g.SetActive(false);
        }
    }

    public void ActivateUI()
    {
        foreach (GameObject g in uis)
        {
            g.SetActive(true);
        }
    }
}
