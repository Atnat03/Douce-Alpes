using System;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
    public Image logoImage;

    private void Update()
    {
        if(id == 13 && lockImage != null)
        {
            logoImage.transform.transform.position = transform.position + (Vector3.up*20f);
        }
    }
}