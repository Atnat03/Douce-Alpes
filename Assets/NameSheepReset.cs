using UnityEngine;
using UnityEngine.UI;

public class NameSheepReset : MonoBehaviour
{
    [SerializeField] private Text field;
    [SerializeField] private InputField value;
    
    [SerializeField] private string[] defaultNames;

    public void OnEnable()
    {
        field.text = "Entré le nom du mouton...";
        value.text = defaultNames[Random.Range(0, defaultNames.Length)];
    }
}
