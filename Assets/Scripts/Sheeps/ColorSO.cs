using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorSO", menuName = "Scriptable Objects/ColorSO")]
public class ColorSO : ScriptableObject
{
    public List<ColorSingleData>  colorData = new();
}

[Serializable]
public class ColorSingleData
{
    public int id;
    public Material material;
}
