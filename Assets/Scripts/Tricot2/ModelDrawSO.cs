using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Patern Model tricot products")]
public class ModelDrawSO : ScriptableObject
{
    public string name;
    public Sprite image;
    public List<ModelDraw> pattern = new();
}

[Serializable]
public class ModelDraw
{
    public List<int> pointsList = new();
}