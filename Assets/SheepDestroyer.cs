using UnityEngine;

public class SheepDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Sheep sheep = other.GetComponent<Sheep>();
        if (sheep != null)
        {
            GameManager.instance.SheepEnterGrange(sheep);
        }
    }
}