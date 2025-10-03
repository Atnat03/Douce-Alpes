using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private GameObject itemPrefab;

    InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
    }

    public void UpdateUI()
    {
		ResetInventory();
        foreach (Article child in inventoryManager.items)
        {
            GameObject item = Instantiate(itemPrefab, grid.transform);
            item.transform.SetParent(grid.transform);
            
            item.transform.GetChild(0).GetComponent<Image>().sprite = child.logo;
        }
    }

	void ResetInventory()
    {
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Destroy(grid.transform.GetChild(i).gameObject);
        }
    }
}
