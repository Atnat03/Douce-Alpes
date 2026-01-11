using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PanneauShop : TouchableObject
{
    public GameObject shopUI;
    public Transform transformCamera;
    public Transform cameraLookTraveling;
    
    public ChangingCamera changeCamera;
    
    [SerializeField] private float moveAmount = 0.66f;
    [SerializeField] private float moveDuration = 0.5f;

    [SerializeField] private Transform[] posCameraPoints;
    [SerializeField] private GameObject buttonQuit;
    
    private Coroutine moveCoroutine;
    
    [SerializeField] MeshRenderer[] meshRendererCancelShadow;
    
    [Header("Arrow")]
    [SerializeField] private GameObject[] centerArrows;
    
    [SerializeField] private GameObject IconInfo;

    private void Start()
    {
        shopUI.SetActive(false);
    }

    public override void TouchEvent()
    {
        if (GameManager.instance.currentCameraState != CamState.Default)
            return;

        if(changeCamera.gameObject.GetComponent<CameraControl>().IsCameraMoving)
            return;
        
        Debug.Log("Touch shop");
        
        if (!GameManager.instance.shopOpen)
        {
            if(TutoManager.instance != null)
                TutoManager.instance.Shop();
            
            IconInfo.SetActive(false);
            OpenUI();
        }
    }

    public void ActivateExclamation()
    {
        IconInfo.SetActive(true);
    }
    
    public void OpenUI()
    {
        shopUI.SetActive(true);
        buttonQuit.SetActive(true);
        GameManager.instance.shopOpen = true;
        GameManager.instance.ChangeCameraState(CamState.Shop);
        GameManager.instance.ChangeCameraPos(transformCamera.position, transformCamera.localEulerAngles, cameraLookTraveling);
    }

    public void CloseUI()
    {
        if (changeCamera.isInTransition)
            return;

        shopUI.SetActive(false);
        buttonQuit.SetActive(false);
        changeCamera.ResetPosition();
        GameManager.instance.shopOpen = false;
    }

    public void GoLeft()
    {
        if (!GameManager.instance.shopOpen) return;

        Vector3 startPos = Camera.main.transform.position;
        Vector3 endPos = posCameraPoints[2].position;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        
        ActivateCenterArrows(false);

        moveCoroutine = StartCoroutine(CameraTranslate(startPos, endPos));
    }

    public void GoRight()
    {
        if (!GameManager.instance.shopOpen) return;

        Vector3 startPos = Camera.main.transform.position;
        Vector3 endPos = posCameraPoints[0].position;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        
        moveCoroutine = StartCoroutine(CameraTranslate(startPos, endPos));
    }

    public void GoCenter()
    {
        if (!GameManager.instance.shopOpen) return;

        Vector3 startPos = Camera.main.transform.position;
        Vector3 endPos = posCameraPoints[1].position;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        
        ActivateCenterArrows(true);

        moveCoroutine = StartCoroutine(CameraTranslate(startPos, endPos));
    }

    void ActivateCenterArrows(bool state)
    {
        foreach (GameObject arrow in centerArrows)
        {
            arrow.SetActive(state);
        }
    }
    
    private IEnumerator CameraTranslate(Vector3 startPos, Vector3 endPos)
    {
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);
            Camera.main.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        Camera.main.transform.position = endPos;
    }
}
