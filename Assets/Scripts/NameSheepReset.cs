using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NameSheepReset : MonoBehaviour
{
    [SerializeField] private Text field;
    [SerializeField] private InputField value;
    
    [SerializeField] private string[] defaultNames;
    
    [SerializeField] private Button createSheep;
    [SerializeField] private Image imageEmpreinte;
    [SerializeField] private CreateSheepButton createSheepButtonScript;

    public void OnEnable()
    {
        field.text = "Entré le nom du mouton...";
        value.text = defaultNames[Random.Range(0, defaultNames.Length)];
        imageEmpreinte.gameObject.SetActive(false);
        createSheep.interactable = true;
        createSheep.onClick.AddListener(CreateSheep);
    }

    public void CreateSheep()
    {
        StartCoroutine(Empreinte());
    }

    IEnumerator Empreinte()
    {
        imageEmpreinte.gameObject.SetActive(true);
        createSheep.interactable = false;
        
        if(Settings.instance.VibrationsActivated)
            Handheld.Vibrate();
        
        yield return new WaitForSeconds(0.5f);
        
        AudioManager.instance.PlaySound(29);
        
        yield return new WaitForSeconds(0.5f);

        SheepBoidManager.instance.CreateSheep();
        gameObject.SetActive(false);
        createSheepButtonScript.gameObject.SetActive(true);
        createSheepButtonScript.Exit();
    }

}
