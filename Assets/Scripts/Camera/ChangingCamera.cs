using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChangingCamera : MonoBehaviour
{
    Vector3 startPosition;
    Vector3 startRotation;
    [SerializeField] private Button quitButton;
    CameraControl control;

    private void Start()
    {
        GameManager.instance.SheepClicked += ChangeCamera;
        GameManager.instance.GrangeClicked += ChangeCamera;
        GameManager.instance.AbreuvoirClicked += ChangeCamera;
        
        control = GetComponent<CameraControl>();
        
        quitButton.gameObject.SetActive(false);
    }

    public void ChangeCamera(Vector3 newPosition, Vector3 rotation)
    {
        control.enabled = false;
        
        quitButton.gameObject.SetActive(true);
        startPosition = gameObject.transform.position;
        startRotation = gameObject.transform.rotation.eulerAngles;
        
        Vector3 targetPosition = new Vector3(newPosition.x, newPosition.y, newPosition.z);

        gameObject.transform.position = targetPosition;
        gameObject.transform.rotation = Quaternion.Euler(rotation);
    }

    public void ResetPosition()
    {
        gameObject.transform.position = startPosition;
        gameObject.transform.rotation = Quaternion.Euler(startRotation);
        
        control.enabled = true;

        GameManager.instance.ResetCamera();
        quitButton.gameObject.SetActive(false);
    }
}