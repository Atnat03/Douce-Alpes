using UnityEngine;
using UnityEngine.UI;

public class SkinButton : MonoBehaviour
{
    private SkinCarousel carousel;
    private RectTransform rect;

    public void Initialize(SkinCarousel carousel)
    {
        this.carousel = carousel;
        rect = GetComponent<RectTransform>();

        Button btn = GetComponent<Button>();
        if (btn == null) btn = gameObject.AddComponent<Button>();

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (carousel != null)
            carousel.OnSkinClicked(rect);
    }
}