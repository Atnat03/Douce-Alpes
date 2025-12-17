using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TricotPage : MonoBehaviour
{
    [SerializeField] private Text titreProduct;
    [SerializeField] private Text laineToDoProduct;
    [SerializeField] private Image logoProduct;
    [SerializeField] private Image background;
    [SerializeField] private ModelDrawSO model;
    [SerializeField] public Button buttonSelect;

    public void Initialize(ModelDrawSO model)
    {
        titreProduct.text = name;
        laineToDoProduct.text = numberTotalWool(model.pattern).ToString();
        logoProduct.sprite = model.image;
        background.sprite = model.background;
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
