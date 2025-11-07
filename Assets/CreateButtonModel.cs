using System;
using UnityEngine;
using UnityEngine.UI;

public class CreateButtonModel : MonoBehaviour
{
    public ModelListeSO data;
    public GameObject buttonPrefab;
    public TricotManager tricotManager;

    private void Start()
    {
        foreach (ModelDrawSO product in data.listeModel)
        {
            Button newButton = Instantiate(buttonPrefab, transform).GetComponent<Button>();
            newButton.transform.name = product.name;
            newButton.onClick.AddListener(()=> tricotManager.InitalizePattern(product));
        }
    }
}
