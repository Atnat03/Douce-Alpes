using UnityEngine;
using UnityEngine.UI;

public class IsSelectGauchBoutique : MonoBehaviour
{
    public Image IsActiveImage;
    public bool isActive = false;

    void Update()
    {
        IsActiveImage.enabled = isActive;
    }
}
