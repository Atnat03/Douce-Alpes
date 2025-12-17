using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Patern Model tricot products")]
public class ModelDrawSO : ScriptableObject
{
    public string name;
    public Sprite image; 
    public Sprite background; 
    public List<ModelDraw> pattern = new();
    public int sellPrice;
    public int unlockPrice;
    public GameObject prefabVisual;
}

[Serializable]
public class ModelDraw
{
    public List<int> pointsList = new();
    public int neededWool;
}