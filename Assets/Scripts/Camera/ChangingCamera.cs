using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChangingCamera : MonoBehaviour
{
    Vector3 startPosition;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        GameManager.instance.SheepClicked += ChangeCamera;
        
        quitButton.gameObject.SetActive(false);
    }

    public void ChangeCamera(Vector3 newPosition)
    {
        quitButton.gameObject.SetActive(true);
        startPosition = transform.position;
        
        Vector3 targetPosition = new Vector3(newPosition.x, newPosition.y + 2, newPosition.z - 5);
        
        transform.position = Vector3.Lerp(targetPosition, targetPosition, 1f);
    }

    public void ResetPosition()
    {
        transform.position = startPosition;

        GameManager.instance.ResetCamera();
        quitButton.gameObject.SetActive(false);
    }
}