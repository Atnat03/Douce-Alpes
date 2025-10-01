using UnityEngine;

public class SheepTonteScene : MonoBehaviour
{
    [SerializeField] GameObject whoolModel;
    
    void Start()
    {
        whoolModel.SetActive(true);
    }
    
    public void Tonte()
    {
        whoolModel.SetActive(false);
    }
}
