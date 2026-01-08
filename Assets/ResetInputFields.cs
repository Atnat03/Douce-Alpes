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

    public void OnSelect(BaseEventData eventData)
    {
        input.Select();
        input.ActivateInputField();
        input.selectionAnchorPosition = 0;
        input.selectionFocusPosition = input.text.Length;
    }
}
