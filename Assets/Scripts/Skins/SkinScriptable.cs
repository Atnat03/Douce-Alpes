using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkinData", menuName = "Skins/SkinScriptable")]
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
}