using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoney : MonoBehaviour
{
    public static PlayerMoney instance;
    
    [Header("Money")]
    [SerializeField] private int currentMoney;
    [SerializeField] Text txtMoney;
    
    [Header("Whool")]
    [SerializeField] private int currentWhool;
    [SerializeField] Text txtWhool;
    
    public BonheurUI bonheurUI;
    
    [Header("Sprite animated")]
    [SerializeField] private GameObject moneySprite;
    [SerializeField] private GameObject woolSprite;

    [SerializeField] private RectTransform moneyFinalTarget;
    [SerializeField] private RectTransform woolFinalTarget;
    
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        txtMoney.text = currentMoney.ToString();
        txtWhool.text = currentWhool.ToString();
    }
    
    //Money
    public void AddMoney(int value, Vector2 pos)
    {
        Debug.Log(value  + " money ajouté");
        currentMoney += value;
        
        bonheurUI.DropCanva(pos, value, moneySprite, moneyFinalTarget.position);
    }
    
    //Laine
    public void AddWhool(int value, Vector2 pos)
    {
        Debug.Log(value  + " whool ajouté");
        
        currentWhool += value;
        
        bonheurUI.DropCanva(pos, value, woolSprite, woolFinalTarget.position);
    }

    public void RemoveMoney(int value)
    {
        currentMoney -= value;
    }

    public void RemoveWhool(int value)
    {
        currentWhool -= value;
    }

    public bool isEnoughtMoney(int value)
    {
        if (currentMoney - value >= 0)
        {
            return true;
        }
        return false;
    }

    public bool isEnoughtWhool(int value)
    {
        if (currentWhool - value >= 0)
        {
            return true;
        }
        return false;
    }
    
    public int CalculateValueWhoolWithTotalHapiness()
    {
        return 0;
    }
}
