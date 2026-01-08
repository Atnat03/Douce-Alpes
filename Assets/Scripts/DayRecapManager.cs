using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DayRecapManager : MonoBehaviour
{
    [SerializeField] GameObject dayRecapPannel;
    [SerializeField] private Text happinessText;
    [SerializeField] private Text moneyText;
    [SerializeField] private Text woolText;
    [SerializeField] private Text numberDay;
    [SerializeField] private Text nextDayButtonTxt;
    
    [SerializeField] private Image barImageHappiness;
    [SerializeField] private Sprite[] heartSprites;
    public Image heartImage;

    [SerializeField]private float maxPercentage;
    [SerializeField]private float currentPercentage;
    [SerializeField] private float happinessAnimDuration = 1.5f;
    [SerializeField] private HappyExplosion happyParticle;
    [SerializeField] private Vector2 posHeart;
    
    [Header("Color")]
    [SerializeField] private Color32 veryLowColor;
    [SerializeField] private Color32 lowColor;
    [SerializeField] private Color32 midColor;
    [SerializeField] private Color32 highColor;
    [SerializeField] private Color32 veryHighColor;

    private void OnEnable()
    {
        dayRecapPannel.SetActive(false);
    }
    
    private void UpdateCursorAndColor()
    {
        barImageHappiness.fillAmount = currentPercentage / 100f;
        barImageHappiness.color = GetColorForValue(currentPercentage);

        happinessText.text = Mathf.RoundToInt(currentPercentage) + " %";
        heartImage.sprite = GetSprite();
    }


    public void Recap(int numberDay, float happiness, int money, int wool, int numberNextDay)
    {
        dayRecapPannel.SetActive(true);
        maxPercentage = Mathf.Clamp(happiness, 0f, 100f);
        moneyText.text = " + "+money;
        woolText.text = " + " + wool;
        this.numberDay.text = "jour " + numberDay;
        nextDayButtonTxt.text = "Passer au jour " + numberNextDay;
        
        currentPercentage = 0f;

        UpdateCursorAndColor();

        StartCoroutine(AnimateHappiness());
    }

    IEnumerator DesactivatePannel()
    {
        dayRecapPannel.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(2f);
        dayRecapPannel.SetActive(false);
    }
    
    public void NextDay()
    {
        StartCoroutine(DesactivatePannel());
        GameData.instance.numberDay++;
        GameData.instance.ResetDayStats();
    }
    
    public Sprite GetSprite()
    {
        return currentPercentage <= 50f ? heartSprites[0] : heartSprites[1];
    }
    
    private Color32 GetColorForValue(float value)
    {
        if (value <= 10f) return veryLowColor;
        if (value <= 25f) return lowColor;
        if (value <= 50f) return midColor;
        if (value <= 75f) return highColor;
        return veryHighColor;
    }
    
    private IEnumerator AnimateHappiness()
    {
        yield return new WaitForSeconds(1.5f);
        
        currentPercentage = 0f;
        float elapsed = 0f;

        while (elapsed < happinessAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / happinessAnimDuration);

            currentPercentage = Mathf.Lerp(0f, maxPercentage, t);
            UpdateCursorAndColor();

            yield return null;
        }

        currentPercentage = maxPercentage;
        UpdateCursorAndColor();

        if (currentPercentage >= 50f)
        {
            happyParticle.PerformParticle();
        }
    }
}
