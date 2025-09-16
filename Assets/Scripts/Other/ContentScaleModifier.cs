using System;
using UnityEngine;

public class ContentScaleModifier : MonoBehaviour
{
    public void SetSize()
    {
        float nbArticle = transform.childCount;
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.offsetMax = new Vector2(nbArticle * 100, rect.offsetMax.y);
    }
}
