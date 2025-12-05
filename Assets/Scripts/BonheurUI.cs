using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BonheurUI : MonoBehaviour
{
    [FormerlySerializedAs("valueImahe")] [FormerlySerializedAs("backgroundImage")] [Header("UI Elements")]
    public Image valueImage;

    public Image heartImage;
    
    [Header("State Values")]
    [Range(0, 1)] public float currentValue = 0f;
    [Range(0, 1)] public float overflowValue = 0f;
    public bool isOverflow = false;

    [Header("Colors")]
    [SerializeField] private Color32 veryLowColor;
    [SerializeField] private Color32 lowColor;
    [SerializeField] private Color32 midColor;
    [SerializeField] private Color32 highColor;
    [SerializeField] private Color32 veryHighColor;
    [SerializeField] private Color32 overflowColor;
    [SerializeField] private Sprite[] heartSprites;

    private void Update()
    {
        UpdateCursorAndColor();
    }

    private void UpdateCursorAndColor()
    {
        valueImage.color = GetColorForValue(currentValue, isOverflow);
        valueImage.fillAmount = currentValue;
        heartImage.sprite = GetSprite();
    }


    public Sprite GetSprite()
    {
        return currentValue <= 0.5f ? heartSprites[0] : heartSprites[1];
    }
    
    private Color32 GetColorForValue(float value, bool overflow)
    {
        if (overflow) return overflowColor;
        if (value <= 0.1f) return veryLowColor;
        if (value <= 0.25f) return lowColor;
        if (value <= 0.5f) return midColor;
        if (value <= 0.75f) return highColor;
        return veryHighColor;
    }
}