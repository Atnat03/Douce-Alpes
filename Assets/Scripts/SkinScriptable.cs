using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class SkinScriptable : ScriptableObject
{
    public List<SkinSkelete> skins = new List<SkinSkelete>();
}

[Serializable]
public class SkinSkelete
{
    public int id;
    public string name;
    public int price;
    public Sprite logo;
    public Material model;
}

