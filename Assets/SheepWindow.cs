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

    private void Awake()
    {
        instance = this;
    }

    public void SetNewCurrentSkin()
    {
        GameManager.instance.GetSheep(sheepId).SetCurrentSkin(skinManager.GetCurrentSkinID());
    }

    public void Initialize(string name, int currentSkin, int sheepId)
    {
        nameText.text = name;
        this.currentSkin = currentSkin;
        
        Debug.Log(this.currentSkin);
        
        this.sheepId = sheepId;
        skinManager.SetCurrentSkin(this.currentSkin);
    }
}
