using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinBuildManager : MonoBehaviour
{
    public List<GameObject> skins;
    public bool isBarriere = false;

    private void OnEnable()
    {
        if(isBarriere)
            SwapSkin(SkinAgency.instance.skinBarriereId);
        else
        {
            SwapSkin(SkinAgency.instance.skinGrangeId);
        }
    }

    public void SwapSkin(int id)
    {
        if (id >= skins.Count)
            return;

        for (int i = 0; i < skins.Count; i++)
        {
            if (i == id)
            {
                skins[i].SetActive(true);
            }
            else
            {
                skins[i].SetActive(false);
            }
        }
    }
}
