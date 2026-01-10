using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class ArticleScriptable : ScriptableObject
{
    public List<Article> articles;
}

[Serializable]
public class Article
{
    public int id;
    public string title;
    public int price;
    public Sprite logo;
    public RareteItem Rarete;
    public ArticleType type;
}

public enum RareteItem
{
    Commum,
    Rare,
    Legendaire
}