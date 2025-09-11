using System;
using UnityEngine;
using UnityEngine.UI;

public class Sheep : TouchableObject
{
    [SerializeField] private string sheepName;

    private bool isHolding = false;

    [SerializeField] private float currentCaressesValue;
    [SerializeField]  private float maxCaressesValue = 100;
    [SerializeField] private float holingTimer;
    private float timer;
    bool startTimer = false;
    
    [Header("References")]
    [SerializeField]  Image currentBonheur;
    [SerializeField]  Text sheepNameTxt;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private GameObject wheelTimerUI;
    [SerializeField] private Image valueWheelTimerImage;
    
    private void Update()
    {
        currentBonheur.fillAmount = currentCaressesValue / maxCaressesValue;
        sheepNameTxt.text = sheepName;
        
        if (isHolding)
        {
            Vector2 screenPos = TouchManager.instance.playerInput.actions["TouchPosition"].ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            {
                transform.position = hit.point + Vector3.up * 0.5f;
            }
        }
        
        
        
        if (startTimer)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                WidowOpen();
            }
        }
    }

    private void Start()
    {
        holingTimer = TouchManager.instance.holdThreshold;
    }

    public Vector3 GetCameraPosition()
    {
        return cameraPosition.transform.position;
    }

    public void AddCaresse()
    {
        currentCaressesValue += 1f;
        GameManager.instance.Caresse();
    }

    public override void TouchEvent()
    {
        print(gameObject.name + " touched");
    }

    public void StartHolding()
    {
        timer = holingTimer;
        startTimer = true;
    }

    public void WidowOpen()
    {
        GameManager.instance.ChangeCameraPos(GetCameraPosition(), cameraPosition.transform.rotation.eulerAngles);
    }

    public void OnTouchEnd()
    {
        isHolding = false;
    }
}