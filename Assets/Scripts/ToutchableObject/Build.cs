using System;
using UnityEngine;

public class Build : TouchableObject
{
    [SerializeField] public GameObject UI;

    void Start()
    {
        UI.SetActive(false);
    }
    
    public override void TouchEvent()
    {
        if (GameManager.instance.shopOpen) return;
        
        if(GameManager.instance.currentCameraState != CamState.Default)
            return;

        GetComponentInChildren<OnBecameInvisibleObject>().ActivateUI();
    }
    
    public void OpenUI()
    {
        UI.SetActive(true);
    }
    
    public void CloseUI()
    {
        UI.SetActive(false);
    }
}
