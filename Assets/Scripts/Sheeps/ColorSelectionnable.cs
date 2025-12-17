using UnityEngine;

public class ColorSelectionnable : MonoBehaviour
{
    public int idColor;

    public void ChangeColor()
    {
        SheepWindow.instance.SetColor(idColor);
    }
}
