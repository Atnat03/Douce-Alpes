using System;
using UnityEngine;
using UnityEngine.UI;

public class BonheurUI : MonoBehaviour
{
    public Slider slider;
    public Image backgroundImage;
    public Image fillImage;

    public float currentValue = 0;

    [Header("Colors")] 
    [SerializeField] private Color32 veryLowColor;
    [SerializeField] private Color32 LowColor;
    [SerializeField] private Color32 midColor;
    [SerializeField] private Color32 hightColor;
    [SerializeField] private Color32 veryHightColor;
    
    private void Update()
    {
        slider.value = currentValue;

        if(slider.value == 0)
            ChangeColor(veryLowColor);
        else if (slider.value > 0 && slider.value <= 0.25)
            ChangeColor(LowColor);
        else if (slider.value > 0.25 && slider.value <= 0.5)
            ChangeColor(LowColor);
        else if (slider.value > .5 && slider.value <= 0.75)
            ChangeColor(hightColor);
        else if (slider.value > 0.75 && slider.value <= 0.9)
            ChangeColor(veryHightColor);
    }

    void ChangeColor(Color32 color)
    {
        backgroundImage.color = color;
        fillImage.color = color;
    }
}
