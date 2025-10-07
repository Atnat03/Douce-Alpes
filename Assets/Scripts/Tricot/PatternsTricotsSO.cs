using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Patterns tricot products")]
public class PatternsTricotsSO : ScriptableObject
{
    public string name;
    public Sprite image;
    public List<SwipeType> pattern = new();
}

