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
        if (backgroundImage == null || cursorImage == null || minPos == null || maxPos == null || overflowPos == null)
            return;

        // Couleur
        Color32 color = GetColorForValue(currentValue, isOverflow);
        backgroundImage.color = color;

        // Positions
        Vector3 startPos = isOverflow ? maxPos.localPosition : minPos.localPosition;
        Vector3 endPos = isOverflow ? overflowPos.localPosition : maxPos.localPosition;
        float value = isOverflow ? overflowValue : currentValue;

        // Clamp de la valeur pour éviter NaN
        if (float.IsNaN(value) || value < 0f) value = 0f;
        if (value > 1f) value = 1f;

        Vector3 newPos = Vector3.Lerp(startPos, endPos, value);

        // Vérification NaN
        if (float.IsNaN(newPos.x) || float.IsNaN(newPos.y) || float.IsNaN(newPos.z))
        {
            Debug.LogWarning($"Cursor position invalid! startPos={startPos}, endPos={endPos}, value={value}");
            newPos = startPos; // fallback
        }

        cursorImage.transform.localPosition = newPos;
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