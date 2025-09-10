using UnityEngine;

public class Sheep : TouchableObject
{
    [SerializeField] private string name;

    public override void TouchEvent()
    {
        GameManager.instance.ChangeCameraPos(transform.position);
    }
}
