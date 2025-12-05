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
    public void AddMoney(int value)
    {
        Debug.Log(value  + " money ajouté");
        currentMoney += value;
    }
    
    //Laine
    public void AddWhool(int value)
    {
        Debug.Log(value  + " whool ajouté");
        
        currentWhool += value;
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
