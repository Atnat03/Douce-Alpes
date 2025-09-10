using UnityEngine;

public class Build : TouchableObject
{
    [SerializeField] Material newMaterial;

    public override void TouchEvent()
    {
        Debug.Log("Build");
        GetComponent<MeshRenderer>().material = newMaterial;
    }
}
