using System;
using UnityEngine;
using System.Collections.Generic;

public class SkinListManager : MonoBehaviour
{
    List<SkinUnit> skinList;

    private Sheep parentSheep;
    
    private void Start()
    {
        parentSheep =  transform.parent.GetComponent<Sheep>();
     
        skinList = new List<SkinUnit>();

        for (int i = 0; i < transform.childCount; i++)
        {
            skinList.Add(transform.GetChild(i).GetComponent<SkinUnit>());
        }
        
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
