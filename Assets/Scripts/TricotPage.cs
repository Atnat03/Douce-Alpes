using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TricotPage : MonoBehaviour
{
    [SerializeField] private Text titreProduct;
    [SerializeField] private Text laineToDoProduct;
    [SerializeField] private Text gainProduct;
    [SerializeField] private Image logoProduct;
    [SerializeField] private Image background;
    [SerializeField] public ModelDrawSO model;
    [SerializeField] public Button buttonSelect;

    public void Initialize(ModelDrawSO model)
    {
        titreProduct.text = model.name;
        laineToDoProduct.text = numberTotalWool(model.pattern).ToString();
        logoProduct.sprite = model.image;
        background.sprite = model.background;
        gainProduct.text = model.sellPrice.ToString();
    }

    public int numberTotalWool(List<ModelDraw> l)
    {
        int total = 0;
        for (int i = 0; i < l.Count; i++)
        {
            total += l[i].neededWool;
        }
        return total;
    }
}
