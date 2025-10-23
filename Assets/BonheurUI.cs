using UnityEngine;
using UnityEngine.UI;

public class BonheurUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image backgroundImage;
    public Image cursorImage;

    [Header("Positions")]
    [SerializeField] private RectTransform minPos;
    [SerializeField] private RectTransform maxPos;
    [SerializeField] private RectTransform overflowPos;

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

    private void Update()
    {
        UpdateCursorAndColor();
    }

    private void UpdateCursorAndColor()
    {
        Color32 color = GetColorForValue(currentValue, isOverflow);
        backgroundImage.color = color;

        Vector3 startPos = isOverflow ? maxPos.localPosition : minPos.localPosition;
        Vector3 endPos = isOverflow ? overflowPos.localPosition : maxPos.localPosition;
        float value = isOverflow ? overflowValue : currentValue;

        cursorImage.transform.localPosition = Vector3.Lerp(startPos, endPos, value);
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