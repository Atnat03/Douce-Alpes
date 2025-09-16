using System;
using System.Collections.Generic;
using UnityEngine;

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
}