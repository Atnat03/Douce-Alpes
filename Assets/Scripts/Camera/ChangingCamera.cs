using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChangingCamera : MonoBehaviour
{
    Vector3 startPosition;
    Vector3 startRotation;
    [SerializeField] private Button quitButton;
    CameraControl control;
    float elapseTime;
    [SerializeField] float timerToTransition = 1f;

    private void Start()
    {
        GameManager.instance.SheepClicked += LockCamOnSheep;
        GameManager.instance.SheepHold += ChangeCamera;
        GameManager.instance.GrangeClicked += ChangeCamera;
        GameManager.instance.AbreuvoirClicked += ChangeCamera;
        
        control = GetComponent<CameraControl>();
        
        quitButton.gameObject.SetActive(false);

        startPosition = transform.position;
        startRotation = transform.rotation.eulerAngles;
    }
    
    private IEnumerator SmoothTransition(Vector3 targetPosition, Vector3 targetRotation, bool reEnableControl = false, bool hideQuitButton = false)
    {
        elapseTime = 0f;
        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;
        Quaternion finalRotation = Quaternion.Euler(targetRotation);

        control.enabled = false;

        if (!hideQuitButton)
            quitButton.gameObject.SetActive(true);

        while (elapseTime < timerToTransition)
        {
            elapseTime += Time.deltaTime;
            float t = elapseTime / timerToTransition;

            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(initialRotation, finalRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = finalRotation;

        if (reEnableControl)
            control.enabled = true;

        if (hideQuitButton)
            quitButton.gameObject.SetActive(false);
    }


    public void ChangeCamera(Vector3 newPosition, Vector3 rotation)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothTransition(newPosition, rotation));
    }
    
    public void LockCamOnSheep(Sheep sheep)
    {
        print(sheep.laine.GetComponent<Outline>());

        CameraFollow cameraFollow = GetComponent<CameraFollow>();
        CameraControl cameraControl = GetComponent<CameraControl>();

        cameraControl.enabled = false;
        cameraFollow.enabled = true;

        cameraFollow.target = sheep.transform;

        StopAllCoroutines();
        StartCoroutine(SmoothTransition(
            sheep.transform.position + cameraFollow.offset,
            transform.rotation.eulerAngles
        ));

        sheep.ChangeOutlineState(true);
    }

    public void ResetCameraLock(Sheep sheep)
    {
        CameraFollow cameraFollow = GetComponent<CameraFollow>();
        CameraControl cameraControl = GetComponent<CameraControl>();
        cameraControl.enabled = true;
        cameraFollow.enabled = false;
        
        sheep.ChangeOutlineState(false);
    }
    
    public void ResetPosition()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothTransition(startPosition, startRotation, reEnableControl: true, hideQuitButton: true));

        GameManager.instance.ResetCamera();
    }
}
