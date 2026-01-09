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
        selectedScale = startScale * 1.1f;
        ChangePage(0);
    }

    public void ChangePage(int id)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            if (i == id)
            {
                pages[i].SetActive(true);
                
                UpdateOnglet(i);
                onglets[i].transform.SetSiblingIndex(3);
            }
            else if (i < id)
            {
                pages[i].SetActive(false);
            }
            else
            {
                pages[i].SetActive(true);
            }
        }
    }

    void UpdateOnglet(int i)
    {
        List<int> order = new List<int>();

        for (int j = 0; j < onglets.Count; j++)
            order.Add(j);

        order.Sort((a, b) => b.CompareTo(a));

        order.Remove(i);
        order.Insert(0, i);

        for (int h = 0; h < order.Count; h++)
        {
            onglets[order[h]].transform.SetSiblingIndex(h);

            if (order[h] == i)
                onglets[order[h]].transform.localScale = selectedScale;
            else
                onglets[order[h]].transform.localScale = startScale;
        }
    }


}
