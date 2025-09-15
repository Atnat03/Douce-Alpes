using System;
using UnityEngine;
using System.Collections.Generic;

public class SkinListManager : MonoBehaviour
{
    [Header("Mettre tout les skins en enfant de l'objet")]
    List<SkinUnit> skinList;

    private Sheep parentSheep;
    
    private void Start()
    {
        skinList = new List<SkinUnit>();

        for (int i = 0; i < transform.childCount; i++)
        {
            skinList.Add(transform.GetChild(i).GetComponent<SkinUnit>());
        }
        
        parentSheep =  transform.parent.GetComponent<Sheep>();
        
        UpdateSkinList(parentSheep.currentSkin);
    }

    public void UpdateSkinList(int currentSkin)
    {
        foreach (SkinUnit skinUnit in skinList)
        {
            skinUnit.gameObject.SetActive(false);

            if (currentSkin == skinUnit.id)
            {
                skinUnit.gameObject.SetActive(true);
            }
        }
    }
}
