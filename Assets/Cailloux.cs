using UnityEngine;

public class Cailloux : TouchableObject
{
    [SerializeField] AudioClip clip;
    [SerializeField] private int idTouch;

    public override void TouchEvent()
    {
        Debug.Log("TouchéCailloux");
        
        base.TouchEvent();

        transform.parent.GetComponent<PianoCailloux>().PlayTouche(clip, idTouch);
    }
}
