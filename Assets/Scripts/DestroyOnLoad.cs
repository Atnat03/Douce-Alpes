using UnityEngine;

public class DestroyOnLoad : MonoBehaviour
{
    public float time;
    
    void Start()
    {
        Destroy(gameObject, time);
    }
}
