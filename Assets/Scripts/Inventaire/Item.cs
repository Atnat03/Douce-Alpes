using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private Sprite icon;
    [SerializeField] private Text name;

    public void Initialize(Sprite icon, Text name)
    {
        this.icon = icon;
        this.name = name;
    }
}
