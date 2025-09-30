using System;
using UnityEngine;
using UnityEngine.UI;

public class ContentScaleModifier : MonoBehaviour
{
    public void SetSize(int nbArticle)
    {
        RectTransform rect = GetComponent<RectTransform>();

        float newHeight = Mathf.CeilToInt((nbArticle / 3f)) * 140f;

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
    }

    public void ResetSize()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = Vector2.zero;
    }
}