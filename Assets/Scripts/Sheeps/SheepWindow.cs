using System;
using UnityEngine;
using UnityEngine.UI;

public class SheepWindow : MonoBehaviour
{
    public static SheepWindow instance;
    
    [SerializeField] SkinManager skinManager;
    
    [Header("UI")] 
    [SerializeField] private InputField nameText;
    private int currentSkinHat;
    private int currentSkinClothe;
    private int sheepId;

    public bool isOpen = false;

    private void Awake()
    {
        instance = this;
    }

    public void SetNewCurrentSkinHat(int id)
    {
        GameManager.instance.GetSheep(sheepId).SetCurrentSkinHat(id);
    }
    
    public void SetNewCurrentSkinClothe(int id)
    {
        GameManager.instance.GetSheep(sheepId).SetCurrentSkinClothe(id);
    }
    
    public InputField GetInputField(){return nameText;}

    public int GetCurrentSheepID()
    {
        return sheepId;
    }

    public void Initialize(string name, int currentSkinHat, int currentSkinClothe, int sheepId)
    {
        isOpen = true;
        
        nameText.text = name;
        this.currentSkinHat = currentSkinHat;
        this.currentSkinClothe = currentSkinClothe;
        
        this.sheepId = sheepId;
    }

    public void ResetValue()
    {
        isOpen = false;
        currentSkinHat = -1;
        currentSkinClothe = -1;
        sheepId = -1;
    }
}
