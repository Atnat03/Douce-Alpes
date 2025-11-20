using System;
using System.Collections.Generic;
using UnityEngine;

public class OngletShopManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages;
    [SerializeField] private List<GameObject> onglets;
    private Vector3 startScale;
    private Vector3 selectedScale;

    private void Start()
    {
        startScale = onglets[0].transform.localScale;
        selectedScale = startScale * 1.25f;
    }

    public void ChangePage(int id)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            if (i == id)
            {
                // On active la page sélectionnée
                pages[i].SetActive(true);

                // On place l'onglet en position 3
                onglets[i].transform.SetSiblingIndex(3);
            }
            else if (i < id)
            {
                // On désactive toutes les pages avant id
                pages[i].SetActive(false);
            }
            else
            {
                // On laisse activées les pages après id
                pages[i].SetActive(true);
            }
        }
    }

}
