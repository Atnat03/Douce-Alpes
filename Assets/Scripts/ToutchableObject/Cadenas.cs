using UnityEngine;

public class Cadenas : TouchableObject
{
    public int hp = 6;

    public override void TouchEvent()
    {
        Debug.Log("Perte d'hp");
        
        hp--;

        if (hp == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
