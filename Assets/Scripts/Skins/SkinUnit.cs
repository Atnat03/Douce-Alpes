using UnityEngine;

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
}