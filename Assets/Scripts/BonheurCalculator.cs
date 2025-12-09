using System;
using UnityEngine;

public class BonheurCalculator : MonoBehaviour
{
    public static BonheurCalculator instance;
    private void Awake() => instance = this;

    [Header("Bonheur")]
    [SerializeField] public float currentBonheur = 0f;
    [SerializeField] public float maxBonheur = 100f;
    [SerializeField] public float overflowMaxValueRatio = 0.2f;
    
    public float overflowValue = 0f;
    private float overflowMaxValue = 0f;
    private float virtualMaxBonheur = 0f;
    private bool isOverflow = false;

    [Header("UI")]
    public BonheurUI bonheurUI;

    [SerializeField] private GameObject animatedSpriteHeart;
    [SerializeField] private RectTransform animatedSpriteHeartTransform;

    private void Start()
    {
        currentBonheur = maxBonheur / 2f;
        overflowMaxValue = maxBonheur * overflowMaxValueRatio;
        virtualMaxBonheur = maxBonheur;
    }

    private void OnEnable()
    {
        if (SheepBoidManager.OnListChanged != null)
            SheepBoidManager.OnListChanged += UpdateMaxBonheur;
    }

    private void OnDisable()
    {
        SheepBoidManager.OnListChanged -= UpdateMaxBonheur;
    }

    void UpdateMaxBonheur(SheepBoid s)
    {
        maxBonheur *= GameData.instance.nbSheep;
    }

    private void Update()
    {
        UpdateBonheurState();
        UpdateUI();
    }

    private void UpdateBonheurState()
    {
        currentBonheur = Mathf.Clamp(currentBonheur - 0.5f * Time.deltaTime, 0, maxBonheur + overflowMaxValue);

        if (currentBonheur > maxBonheur)
        {
            isOverflow = true;
            overflowValue = Mathf.Clamp(currentBonheur - maxBonheur, 0, overflowMaxValue);
            virtualMaxBonheur = overflowMaxValue;
        }
        else
        {
            isOverflow = false;
            overflowValue = 0;
            virtualMaxBonheur = maxBonheur;
        }
    }

    private void UpdateUI()
    {
        if (bonheurUI == null) return;

        bonheurUI.isOverflow = isOverflow;
        bonheurUI.currentValue = isOverflow ? 1f : currentBonheur / maxBonheur;
        bonheurUI.overflowValue = isOverflow ? overflowValue / overflowMaxValue : 0f;
    }

    public void UpdateBonheurValues()
    {
        maxBonheur = 100f * GameData.instance.nbSheep;
        overflowMaxValue = maxBonheur * overflowMaxValueRatio;
        virtualMaxBonheur = isOverflow ? overflowMaxValue : maxBonheur;
        UpdateUI();
    }

    public void AddBonheur(Vector2 pos, float value = 20f)
    {
        currentBonheur = Mathf.Min(currentBonheur + value, maxBonheur + overflowMaxValue);
        bonheurUI.StartAnimatedSprite(pos, 10, animatedSpriteHeart, animatedSpriteHeartTransform.position);
    }

    public void RemoveBonheur(float value = 20f)
    {
        currentBonheur = Mathf.Max(currentBonheur - value, 0);
    }
}
