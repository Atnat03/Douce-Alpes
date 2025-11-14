using UnityEngine;

public class SheepSkinManager : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private SkinListManagerInterior skinListManager;

    [Header("Skin actuelle du mouton")]
    [SerializeField] private int currentSkinHat = 0;
    [SerializeField] private int currentSkinClothe = 0;

    [Header("Infos mouton")]
    [SerializeField] private int sheepId;
    [SerializeField] private string sheepName;
    
    public void Initialize(int id, string name)
    {
        sheepId = id;
        sheepName = name;
    }
    
    public void SetCurrentSkinHat(int skinId)
    {
        currentSkinHat = skinId;
        if (skinListManager != null)
            skinListManager.UpdateSkinListHat(currentSkinHat);
    }
    
    public void SetCurrentSkinClothe(int skinId)
    {
        currentSkinClothe = skinId;
        if (skinListManager != null)
            skinListManager.UpdateSkinListClothe(currentSkinClothe);
    }
    
    public int GetCurrentSkinHat()
    {
        return currentSkinHat;
    }

    public int GetCurrentSkinClothe()
    {
        return currentSkinClothe;
    }


    public string GetSheepName()
    {
        return sheepName;
    }
    
    public int GetSheepId()
    {
        return sheepId;
    }
}