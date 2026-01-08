using System;
using UnityEngine;

public class DLC : MonoBehaviour
{
    public GameObject pageDLC;
    public bool isSelect = false;

    public Action ChangeSelect;

    public void SelectAndUnSelect()
    {
        isSelect = !isSelect;
        ChangeSelect?.Invoke();
    }

    public void ActivateDLC(bool state)
    {
        pageDLC.SetActive(state);
    }
    
    
}
