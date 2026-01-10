using System;
using System.Collections;
using UnityEngine;

public class Build : TouchableObject
{
    [SerializeField] public GameObject UI;
    [SerializeField] public GameObject exclamation;

    void Start()
    {
        UI.SetActive(false);
    }

    public void ActivateExclamation()
    {
        exclamation.SetActive(true);
    }
    
    public override void TouchEvent()
    {
        if (GameManager.instance.shopOpen) return;
        
        if(GameManager.instance.currentCameraState != CamState.Default)
            return;

        GetComponentInChildren<OnBecameInvisibleObject>().ActivateUI();
        
        exclamation.SetActive(false);
    }
    
    public void OpenUI()
    {
        if (Camera.main.GetComponent<CameraControl>().IsCameraMoving)
            return;
        
        if (UI.activeInHierarchy)
            return;
        
        UI.SetActive(true);
        
        AudioManager.instance.PlaySound(24);

        StopAllCoroutines();
        StartCoroutine(CloseDelay());
    }

    IEnumerator CloseDelay()
    {
        yield return new WaitForSeconds(3f);
        CloseUI();
    }
    
    public void CloseUI()
    {
        UI.SetActive(false);
    }
}
