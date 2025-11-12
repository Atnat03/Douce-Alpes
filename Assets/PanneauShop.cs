using System;
using System.Collections;
using UnityEngine;

public class PanneauShop : TouchableObject
{
    public GameObject shopUI;
    public Transform transformCamera;
    public Transform cameraLookTraveling;
    
    public ChangingCamera changeCamera;
    
    [SerializeField] private float moveAmount = 0.66f;
    [SerializeField] private float moveDuration = 0.5f;

    [SerializeField] private Transform[] posCameraPoints;
    
    private Coroutine moveCoroutine;

    private void Start()
    {
        shopUI.SetActive(false);
    }

    public override void TouchEvent()
    {
        if (GameManager.instance.currentCameraState != CamState.Default)
            return;
        
        Debug.Log("Touch shop");
        
        if (!GameManager.instance.shopOpen)
        {
            OpenUI();
        }
    }

    void OpenUI()
    {
        shopUI.SetActive(true);
        GameManager.instance.shopOpen = true;
        GameManager.instance.ChangeCameraState(CamState.Shop);
        GameManager.instance.ChangeCameraPos(transformCamera.position, transformCamera.localEulerAngles, cameraLookTraveling);
    }

    public void CloseUI()
    {
        shopUI.SetActive(false);
        GameManager.instance.shopOpen = false;
        GameManager.instance.ChangePlayerEnvironnement(true);
        changeCamera.ResetPosition();
    }

    public void GoLeft()
    {
        if (!GameManager.instance.shopOpen) return;

        Vector3 startPos = Camera.main.transform.position;
        Vector3 endPos = posCameraPoints[2].position;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

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

        moveCoroutine = StartCoroutine(CameraTranslate(startPos, endPos));
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
