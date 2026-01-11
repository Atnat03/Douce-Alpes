using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResetInputFields : MonoBehaviour
{
    InputField input;

    void Awake()
    {
        input = GetComponent<InputField>();
    }

    public void OnClick()
    {
        print("click");
        input.text = "";
        input.ActivateInputField();
    }
}