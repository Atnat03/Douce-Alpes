using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerMoney : MonoBehaviour
{
    public static PlayerMoney instance;
    
    [Header("Money")]
    [SerializeField] public int currentMoney;
    [SerializeField] Text txtMoney;
    
    [Header("Whool")]
    [SerializeField] public int currentWhool;
    [SerializeField] Text txtWhool;
    
    public BonheurUI bonheurUI;
    
    [Header("Sprite animated")]
    [SerializeField] private GameObject moneySprite;
    [SerializeField] private GameObject woolSprite;

    [SerializeField] private RectTransform moneyFinalTarget;
    [SerializeField] private RectTransform woolFinalTarget;
    
    [SerializeField] private int[] sheepsPrices;
    
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        txtMoney.text = currentMoney.ToString();
        txtWhool.text = currentWhool.ToString();
    }

    public void AddMoneyCheat()
    {
        currentMoney += 100;
    }
    public void AddWoolCheat()
    {
        currentWhool += 100;
    }
    
    
    //Money
    public void AddMoney(int value, Vector2 pos)
    {
        Debug.Log(value  + " money ajouté");
        
        bonheurUI.DropCanva(pos, value, moneySprite, moneyFinalTarget.position);
        
        AudioManager.instance.PlaySound(8, Random.Range(0.95f, 1.05f), 0.2f);

        StartCoroutine(AddMoneySmooth(currentMoney + value));
    }

    IEnumerator AddMoneySmooth(int finalValue)
    {
        yield return new WaitUntil(() => bonheurUI.IsSpawnAnimationFinished());

        while (currentMoney < finalValue)
        {
            currentMoney += 1;
            yield return null;
        }

        currentMoney = finalValue;
        
        GameData.instance.currentMoneyDay += finalValue;

        bonheurUI.RemonteCanva();
    }

    
    //Laine
    public void AddWhool(int value, Vector2 pos)
    {
        Debug.Log(value  + " whool ajouté");
        
        bonheurUI.DropCanva(pos, value, woolSprite, woolFinalTarget.position);
        
        AudioManager.instance.PlaySound(35);
        
        StartCoroutine(AddWoolSmooth(currentWhool + value));
    }
    
    IEnumerator AddWoolSmooth(int finalValue)
    {
        yield return new WaitUntil(() => bonheurUI.IsSpawnAnimationFinished());

        while (currentWhool < finalValue)
        {
            currentWhool += 1;
            yield return null;
        }

        currentWhool = finalValue;
        GameData.instance.currentWoolDay += finalValue;

        bonheurUI.RemonteCanva();
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

    public void LoadStats(int gold, int wool)
    {
        currentMoney = gold;
        currentWhool = wool;
    }
    
    public int CalculateValueWhoolWithTotalHapiness()
    {
        return 0;
    }

    public int GetCurrentSheepPrice()
    {
        return sheepsPrices[GameData.instance.nbSheep];
    }
}
