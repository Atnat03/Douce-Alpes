using UnityEngine;

public class SheepDestroyer : MonoBehaviour
{
    [SerializeField] private GameObject particlePouf;
    
    private void OnTriggerEnter(Collider other)
    {
        Sheep sheep = other.GetComponent<Sheep>();
        if (sheep != null)
        {
            Instantiate(particlePouf, sheep.transform.position, sheep.transform.rotation);
            GameManager.instance.SheepEnterGrange(sheep);
        }
    }
}