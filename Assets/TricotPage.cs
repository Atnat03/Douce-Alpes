using UnityEngine;
using UnityEngine.UI;

public class TricotPage : MonoBehaviour
{
    [SerializeField] private Text titreProduct;
    [SerializeField] private Text priceProduct;
    [SerializeField] private Image logoProduct;

    public void Initialize(string name, int price, Sprite sprite)
    {
        titreProduct.text = name;
        priceProduct.text = price.ToString();
        logoProduct.sprite = sprite;
    }
}
