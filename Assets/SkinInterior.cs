using System.Collections.Generic;
using UnityEngine;

public class SkinInterior : MonoBehaviour
{
    public GameObject[] skins;

    void OnEnable()
    {
        ActivateSkin();
    }
    
    public void ActivateSkin()
    {
        foreach (KeyValuePair<int, bool> kvp in SkinAgency.instance.dicoInteriorSkin)
        {
            int id = kvp.Key;
            bool isActive = kvp.Value;
            
            skins[id].SetActive(isActive);
        }
    }

}
