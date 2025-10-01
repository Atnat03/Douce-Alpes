using System;
using UnityEngine;

public class Cadenas : TouchableObject
{
    public int hp = 2;
    [HideInInspector]public int maxHp;

    private void Start()
    {
        maxHp = hp;
    }

    public override void TouchEvent()
    {
        Debug.Log("Perte d'hp");
        
        hp--;

        if (hp == 0)
        {
            Debug.Log("Destroy candena");
            gameObject.SetActive(false);
        }
    }
}
