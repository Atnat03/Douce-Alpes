using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChangingCamera : MonoBehaviour
{
    Vector3 startPosition;
    Vector3 startRotation;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        GameManager.instance.SheepClicked += ChangeCamera;
        GameManager.instance.GrangeClicked += ChangeCamera;
        
        quitButton.gameObject.SetActive(false);
    }

    public void ChangeCamera(Vector3 newPosition, Vector3 rotation)
    {
        quitButton.gameObject.SetActive(true);
        startPosition = transform.position;
        startRotation = transform.rotation.eulerAngles;
        
        Vector3 targetPosition = new Vector3(newPosition.x, newPosition.y, newPosition.z);

        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(rotation);
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(startRotation);

        GameManager.instance.ResetCamera();
        quitButton.gameObject.SetActive(false);
    }
}