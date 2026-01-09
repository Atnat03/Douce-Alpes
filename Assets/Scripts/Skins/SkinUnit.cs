using System;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SkinCombo
{
    Default,
    Sherif,
    Leader,
    Chill,
    Artiste,
    Intelo,
}


public class SkinUnit : MonoBehaviour
{
    public int id;
    public SkinCombo combo;
    public ArticleType type;
    public bool canPutThisSkin = false;
    [SerializeField] public TextMeshProUGUI stackText;
    public Image lockImage;
}