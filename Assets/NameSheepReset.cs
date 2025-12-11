using UnityEngine;
using UnityEngine.UI;

public class NameSheepReset : MonoBehaviour
{
    [SerializeField] private Text field;
    [SerializeField] private InputField value;

    public void OnEnable()
    {
        value.text = "";
        field.text = "Nom du mouton...";
    }
}
