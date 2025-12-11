using UnityEngine;

public class SheepTonteScene : MonoBehaviour
{
    [SerializeField] GameObject whoolModel;
    
    [SerializeField] Material[] sheepMaterials;
    
    void Start()
    {
        whoolModel.SetActive(true);
    }
    
    public void Tonte()
    {
        whoolModel.SetActive(false);
    }

    public void Initialize(int colorId)
    {
        whoolModel.GetComponent<Renderer>().material = sheepMaterials[colorId];
    }
}
