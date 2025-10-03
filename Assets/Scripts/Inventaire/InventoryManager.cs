using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    private void Awake() { instance = this; }

    InventoryUI inventoryUI;
    [SerializeField] private GameObject ui;
    public int maxItemAuthorized = 5;
    public int currrentItemInInventory = 0;
    
    [SerializeField] public List<Article> items;

    private void Start()
    {
        inventoryUI = GetComponent<InventoryUI>();
        
        CloseUI();
    }

    public void AddItem(Article item)
    {
        if (currrentItemInInventory + 1 > maxItemAuthorized) return;

        currrentItemInInventory++;
        
        items.Add(item);

        inventoryUI.UpdateUI();
    }
    
    public void OpenUI()
    {
        ui.SetActive(true);
    }

    public void CloseUI()
    {
        ui.SetActive(false);
    }
}
