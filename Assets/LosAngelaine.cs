using UnityEngine;

public class LosAngelaine : MonoBehaviour
{
    public GameObject soon;
    
    public void Click()
    {
        GameObject s = Instantiate(soon, transform);
        Destroy(s, 2f);
    }
}
