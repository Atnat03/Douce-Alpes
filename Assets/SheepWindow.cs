using System;
using UnityEngine;
using UnityEngine.UI;

public class SheepWindow : MonoBehaviour
{
    public static SheepWindow instance;
    
    [SerializeField] SkinManager skinManager;
    
    [Header("UI")] 
    [SerializeField] private InputField nameText;
    private int currentSkin;
    private int sheepId;

    public bool isOpen = false;

    private void Awake()
    {
        instance = this;
    }

    public void SetNewCurrentSkin()
    {
        GameManager.instance.GetSheep(sheepId).SetCurrentSkin(skinManager.GetCurrentSkinID(), skinManager.GetCurrentSkinModel());
    }

    public int GetCurrentSheepID()
    {
        return sheepId;
    }

    public void Initialize(string name, int currentSkin, int sheepId)
    {
        isOpen = true;
        
        nameText.text = name;
        this.currentSkin = currentSkin;
        
        Debug.Log(this.currentSkin);
        
        this.sheepId = sheepId;
        skinManager.SetCurrentSkin(this.currentSkin);
    }

    public void ResetValue()
    {
        isOpen = false;
        currentSkin = -1;
        sheepId = -1;
    }
}
