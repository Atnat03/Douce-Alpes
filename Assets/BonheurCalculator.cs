using System;
using UnityEngine;
using UnityEngine.UI;

public class BonheurCalculator : MonoBehaviour
{
    public static BonheurCalculator instance;    private void Awake() { instance = this; }
    
    [Header("Bonheur")] 
    [SerializeField] public float currentBonheur = 0;
    [SerializeField] public float maxBonheur = 100;
    public BonheurUI bonheurUI;

    private void Start()
    {
        maxBonheur = maxBonheur * GameData.instance.nbSheep;
        currentBonheur = maxBonheur / 2;
    }

    public void UpdateBonheur()
    {
        if(bonheurUI.slider != null)
        {
            bonheurUI.currentValue = (currentBonheur / maxBonheur);
        }
    }

    void Update()
    {
        currentBonheur -= 0.5f * Time.deltaTime;

        if (currentBonheur < 0) currentBonheur = 0;
        if(currentBonheur >= maxBonheur) currentBonheur = maxBonheur;
        
        UpdateBonheur();
    }

    public void AddBonheur()
    {
        currentBonheur += 20;
    }

    public void RemoveBonheur()
    {
        currentBonheur -= 20;
    }
}
